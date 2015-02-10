using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shoy.Laboratory
{
    /// <summary>离散型马尔可夫链预测模型</summary>
    public class DiscreteMarkov
    {
        #region 属性

        /// <summary>样本点状态时间序列,按照时间升序</summary>
        public List<int> StateList { get; set; }

        /// <summary>状态总数,对应模型的m</summary>
        public int Count { get; set; }

        /// <summary>概率转移矩阵Pij</summary>
        public List<DenseMatrix> ProbMatrix { get; set; }

        /// <summary>各阶的自相关系数</summary>
        public double[] Rk { get; set; }

        /// <summary>各阶的权重</summary>
        public double[] Wk { get; set; }

        /// <summary>频数矩阵</summary>
        public int[][] CountStatic { get; set; }

        /// <summary>目标序列是否满足"马氏性"</summary>
        public Boolean IsMarkov { get; set; }

        /// <summary>滞时期，K</summary>
        public int LagPeriod { get; set; }

        /// <summary>预测概率</summary>
        public double[] PredictValue { get; set; }

        #endregion

        #region 构造函数

        public DiscreteMarkov(List<int> data, int count, int k = 5)
        {
            StateList = data;
            LagPeriod = k;
            Count = count;
            CountStatic = StaticCount(data, count);
            ProbMatrix = new List<DenseMatrix>();
            var t0 = DenseMatrix.OfArray(StaticProbability(CountStatic).ConvertToArray());
            ProbMatrix.Add(t0);

            for (int i = 1; i < k; i++) //根据CK方程，计算各步的状态转移矩阵
            {
                var temp = ProbMatrix[i - 1] * t0;
                ProbMatrix.Add(temp);
            }
            IsMarkov = ValidateMarkov();
            if (IsMarkov)
            {
                CorrCoefficient();
                TimeWeight();
                PredictProb();
            }
            else
            {
                Console.WriteLine("马氏性 检验失败,无法进行下一步预测");
            }
        }

        #endregion

        #region 验证

        /// <summary>验证是否满足马氏性,默认的显著性水平是0.05，自由度25</summary>
        /// <returns></returns>
        private bool ValidateMarkov()
        {
            //计算列和
            var cp1 = new int[Count];
            int allcount = CountStatic.Select(n => n.Sum()).Sum(); //总数

            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++) cp1[i] += CountStatic[j][i];
            }
            double[] cp = cp1.Select(n => (double)n / (double)allcount).ToArray();

            //计算伽马平方统计量
            double gm = 0;
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < Count; j++)
                {
                    if (CountStatic[i][j] != 0)
                        gm += 2 * CountStatic[i][j] * Math.Abs(Math.Log(ProbMatrix[0][i, j] / cp[j], Math.E));
                }
            }
            //查表求a = 0.05时，伽马分布的临界值F(m-1)^2,如果实际的gm值大于差别求得的值，则满足
            //查表要自己做表，这里只演示0.05的情况  卡方分布            
            return gm >= 37.65;
        }

        /// <summary>计算相关系数</summary>
        private void CorrCoefficient()
        {
            double mean = StateList.Sum() / (double)StateList.Count; //均值

            double p = StateList.Select(n => (n - mean) * (n - mean)).Sum();

            Rk = new double[LagPeriod];

            for (int i = 0; i < LagPeriod; i++)
            {
                double s1 = 0;
                for (int l = 0; l < StateList.Count - LagPeriod; l++)
                {
                    s1 += (StateList[l] - mean) * (StateList[l + i] - mean);
                }
                Rk[i] = s1 / p;
            }
        }

        /// <summary>计算滞时的步长</summary>
        private void TimeWeight()
        {
            double sum = Rk.Select(Math.Abs).Sum();
            Wk = Rk.Select(n => Math.Abs(n) / sum).ToArray();
        }

        /// <summary>预测状态概率</summary>
        private void PredictProb()
        {
            PredictValue = new double[Count];
            //这里很关键，权重和滞时的关系要颠倒，循环计算的时候要注意
            //另外，要根据最近几期的出现数，确定概率的状态，必须取出最后几期的数据

            //1.先取最后K期数据
            var last = StateList.GetRange(StateList.Count - LagPeriod, LagPeriod);
            //2.注意last数据是升序,最后一位对于的滞时期 是k =1 
            for (int i = 0; i < Count; i++)
            {
                for (int j = 0; j < LagPeriod; j++)
                {
                    //滞时期j的数据状态
                    var state = last[last.Count - 1 - j] - 1;
                    PredictValue[i] += Wk[j] * ProbMatrix[j][state, i];
                }
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>统计频数矩阵</summary>
        /// <param name="data">升序数据</param>
        /// <param name="statusCount"></param>
        private int[][] StaticCount(List<int> data, int statusCount)
        {
            var res = new int[statusCount][];

            for (int i = 0; i < statusCount; i++) res[i] = new int[statusCount];

            for (int i = 0; i < data.Count - 1; i++) res[data[i] - 1][data[i + 1] - 1]++;

            return res;
        }

        /// <summary>根据频数，计算转移概率矩阵</summary>
        /// <param name="data">频率矩阵</param>
        private double[][] StaticProbability(int[][] data)
        {
            var res = new double[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                int sum = data[i].Sum();
                res[i] = data[i].Select(n => (double)n / (double)sum).ToArray();
            }
            return res;
        }

        #endregion
    }

    public static class DiscreteMarkovExtend
    {
        public static T[,] ConvertToArray<T>(this T[][] data)
        {
            var res = new T[data.Length, data[0].Length];
            for (int i = 0; i < data.Length; i++)
            {
                for (var j = 0; j < data[i].Length; j++)
                    res[i, j] = data[i][j];
            }
            return res;
        }
    }
}
