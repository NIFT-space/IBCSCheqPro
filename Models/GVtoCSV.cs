using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class GVtoCSV
    {
        public GVtoCSV()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public StringBuilder ExportGridToCSV(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sb.Append(dt.Columns[i]);
                if (i < dt.Columns.Count - 1)
                {
                    sb.Append(",");
                }
            }
            sb.Append("\r\n");
            foreach (DataRow dr in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sb.Append(value);
                        }
                        else
                        {
                            sb.Append(dr[i].ToString());
                        }
                    }
                    if (i < dt.Columns.Count - 1)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("\r\n");
            }
            //IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
            //                                  Select(column => column.ColumnName);
            //sb.AppendLine(string.Join(",", columnNames));

            //foreach (DataRow row in dt.Rows)
            //{
            //    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
            //    sb.AppendLine(string.Join(",", fields));
            //}
            //for (int k = 0; k < gv.Columns.Count; k++)
            //{

            //    sb.Append(gv.Columns[k].HeaderText + ',');
            //}

            //sb.Append("\r\n");
            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    for (int k = 0; k < dt.Columns.Count; k++)
            //    {

            //        sb.Append(dt.Rows[i][k].ToString() + ',');
            //    }

            //    sb.Append("\r\n");
            //}
            return sb;

        }
    }
}