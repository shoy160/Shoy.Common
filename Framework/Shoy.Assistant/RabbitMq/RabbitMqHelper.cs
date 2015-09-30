using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using Shoy.Assistant.Config;
using Shoy.Utility.Config;
using Shoy.Utility.Extend;

namespace Shoy.Assistant.RabbitMq
{
    public class RabbitMqHelper : IDisposable
    {
        private readonly IConnection _connection;
        private IModel _channel;

        public RabbitMqHelper()
        {
            var cf = GetConnection();
            _connection = cf.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel Channel
        {
            get
            {
                if (_channel.IsClosed)
                    _channel = _connection.CreateModel();
                return _channel;
            }
        }

        /// <summary>
        /// 批量发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="infos"></param>
        /// <param name="queue"></param>
        /// <param name="args"></param>
        public void Send<T>(List<T> infos, string queue, IDictionary<string, object> args = null)
        {
            foreach (var info in infos)
            {
                Send(info, queue, args);
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="queue"></param>
        /// <param name="args"></param>
        public void Send<T>(T t, string queue, IDictionary<string, object> args = null)
        {
            if (_channel.IsClosed)
                _channel = _connection.CreateModel();
            //var consumer = new QueueingBasicConsumer(_channel);
            _channel.QueueDeclare(queue, true, false, false, args);
            var message = t.ToJson();
            var body = Encoding.UTF8.GetBytes(message);
            //持久化处理
            IBasicProperties properties = _channel.CreateBasicProperties();
            properties.DeliveryMode = 2;
            //properties.Expiration = "2000";
            _channel.BasicPublish(string.Empty, queue, properties, body);
        }

        /// <summary>
        /// 获取队列结果
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public BasicGetResult GetResult(string queue)
        {
            if (_channel.IsClosed)
                _channel = _connection.CreateModel();
            return _channel.BasicGet(queue, false);
        }

        /// <summary>
        /// 删除队列
        /// </summary>
        /// <param name="deliveryTag"></param>
        public void RemoveQueue(ulong deliveryTag)
        {
            if (_channel.IsClosed)
                _channel = _connection.CreateModel();
            _channel.BasicAck(deliveryTag, false);
        }

        /// <summary>
        /// 获取消息，并将消息从队列中清除
        /// </summary>
        public T Receive<T>(string queue)
            where T : new()
        {
            bool continu;
            return Receive<T>(queue, out continu);
        }

        /// <summary>
        /// 获取消息，并将消息从队列中清除
        /// </summary>
        public T Receive<T>(string queue, out bool continu)
            where T : new()
        {
            continu = true;
            var result = default(T);
            try
            {
                //noAck = true，不需要回复，接收到消息后，queue上的消息就会清除
                //noAck = false，需要回复，接收到消息后，queue上的消息不会被清除，
                // 直到调用channel.basicAck(deliveryTag, false); queue上的消息才会被清除
                //而且，在当前连接断开以前，其它客户端将不能收到此queue上的消息
                if (_channel.IsClosed)
                    _channel = _connection.CreateModel();
                var res = _channel.BasicGet(queue, false);
                if (res == null || res.Body == null || res.Body.Length <= 0)
                {
                    _channel.QueueDelete(queue, false, true);
                    continu = false;
                    return result;
                }
                var message = Encoding.UTF8.GetString(res.Body);
                try
                {
                    return message.JsonToObject<T>();
                }
                catch
                {
                    return result;
                }
                finally
                {
                    _channel.BasicAck(res.DeliveryTag, false);
                }

            }
            catch
            {
                continu = false;
                return result;
            }
        }

        /// <summary>
        /// 获取队列列表，获取之后清空队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<T> ReceiveList<T>(string queue, int size)
            where T : new()
        {
            var list = new List<T>();
            var def = default(T);
            for (int i = 0; i < size; i++)
            {
                bool continu;
                var item = Receive<T>(queue, out continu);
                if (item != null && !item.Equals(def))
                {
                    list.Add(item);
                }
                if (!continu)
                    break;
            }
            return list;
        }

        /// <summary>
        /// 获取消息数量
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        public int MessageCount(string queue)
        {
            if (_channel.IsClosed)
                _channel = _connection.CreateModel();
            var result = _channel.QueueDeclare(queue, true, false, false, null);
            return (int)result.MessageCount;
        }

        private ConnectionFactory GetConnection()
        {
            var config = ConfigUtils<RabbitMqConfig>.Config.Default;
            if (config == null)
                return null;
            return new ConnectionFactory
            {
                UserName = config.UserName,
                Password = config.UserPwd,
                VirtualHost = config.VHost,
                RequestedHeartbeat = 0,
                Endpoint = new AmqpTcpEndpoint(new Uri(config.Url))
            };
        }

        public void Dispose()
        {
            if (_channel != null)
                _channel.Dispose();
            if (_connection != null)
                _connection.Dispose();
        }
    }
}