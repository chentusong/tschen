using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ExcelOperator
{
    public static DataSet ImportData(string filePath)
    {
        try
        {
            DataSet ds = new DataSet();
            Workbook workbook = new Workbook(filePath);

            for (int index = 0; index < workbook.Worksheets.Count; index++)
            {
                Cells cells = workbook.Worksheets[index].Cells;
                // 有标题
                DataTable dt = new DataTable();
                if (cells.MaxDataRow >= 0 && cells.MaxColumn >= 0)
                {
                    dt = cells.ExportDataTable(
                        0,
                        0,
                        cells.MaxDataRow + 1,
                        cells.MaxColumn + 1,
                        true);
                }
                dt.TableName = workbook.Worksheets[index].Name;
                ds.Tables.Add(dt);
            }

            return ds;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + "\r\n" + ex.StackTrace);
        }
    }

    public static void ExportData(DataTable dt, string path)
    {
        if (dt == null)
        {
            return;
        }


        Workbook workbook = new Workbook(); //工作簿
        Worksheet sheet = workbook.Worksheets[0]; //工作表
        sheet.Name = "属性";
        Cells cells = sheet.Cells;//单元格

        //为标题设置样式    
        Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式
        styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        styleTitle.Font.Name = "宋体";//文字字体
        styleTitle.Font.Size = 18;//文字大小
        styleTitle.Font.IsBold = true;//粗体

        //样式2
        Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式
        style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        style2.Font.Name = "宋体";//文字字体
        style2.Font.Size = 14;//文字大小
        style2.Font.IsBold = true;//粗体
        style2.IsTextWrapped = true;//单元格内容自动换行
        style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

        //样式3
        Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式
        style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        style3.Font.Name = "宋体";//文字字体
        style3.Font.Size = 12;//文字大小
        style3.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
        style3.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
        style3.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
        style3.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

        int Colnum = dt.Columns.Count;//表格列数
        int Rownum = dt.Rows.Count + 1;//表格行数


        //生成行1 列名行
        for (int i = 0; i < Colnum; i++)
        {
            cells[0, i].PutValue(dt.Columns[i].ColumnName);
            cells[0, i].SetStyle(style2);
            cells.SetRowHeight(0, 25);
        }

        //生成数据行
        for (int i = 1; i < Rownum; i++)
        {
            for (int k = 0; k < Colnum; k++)
            {

                cells[i, k].PutValue(dt.Rows[i - 1][k].ToString());
                cells[i, k].SetStyle(style3);
            }
            cells.SetRowHeight(1 + i, 24);
        }

        workbook.Save(path);
    }

}