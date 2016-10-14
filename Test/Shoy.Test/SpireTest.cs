using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spire.Doc;
using Spire.Doc.Formatting;

namespace Shoy.Test
{
    [TestClass]
    public class SpireTest
    {
        [TestMethod]
        public void DocTest()
        {
            const string fileName = "shay.pdf";
            var doc = new Document();
            var section = doc.AddSection();
            //            var table = section.AddTable(false);
            //            var row = table.AddRow();
            //            var cell = row.AddCell();
            var baseArea = section.AddParagraph();
            var rang = baseArea.AppendText("试卷标题123456【A卷】");
            rang.CharacterFormat.FontSize = 14;
            rang.CharacterFormat.Bold = true;
            rang.CharacterFormat.FontName = "宋体";
            doc.SaveToFile(fileName, FileFormat.PDF);
            Process.Start(fileName);
        }
    }
}
