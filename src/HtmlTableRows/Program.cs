
using Starcounter;
using Starcounter.Hosting;
using Starcounter.ObjectView;
using Starcounter.Query;
using System.Text;

class Program : IQueryRowsResponse {
    ViewReader reader = new ViewReader(new HtmlFormatter());

    static void Main() {
        var p = new Program();
        var services = Application.Current.Host.GetServiceContainer();
        services.Register<IQueryRowsResponse>(p);

        Handle.GET("/object/{?}", (string oid) => {
            return p.RefClick(oid);
        });
    }

    public Response Respond<T>(QueryResultRows<T> rows) {
        var content = BuildResponseString(rows);

        return new Response() {
            Body = content,
            StatusCode = 200
        };
    }
    
    string BuildResponseString<T>(QueryResultRows<T> rows) {
        var b = new StringBuilder();
        b.Append("<html>");
        b.Append("<head><style>table, th, td { border: 1px solid black; padding: 5px; } table { border - spacing: 5px; }</style ></head>");
        b.Append("<body>");
        b.Append("<table>");
        
        int count = 0;
        foreach (var r in rows) {
            var view = r as IObjectView;
            var tb = view.TypeBinding;

            if (count == 0) {
                // Define column headers
                b.Append("<tr>");
                for (int i = 0; i < tb.PropertyCount; i++) {
                    var p = tb.GetPropertyBinding(i);
                    b.AppendFormat("<th>{0}</th>", p.Name);
                }
                b.Append("</tr>");
            }

            count++;
            b.Append("<tr>");
            
            reader.AllValues(view, (prop, name, value) => {
                b.AppendFormat("<td>{0}</td>", value);
            });
            
            b.Append("</tr>");
        }

        b.Append("</table>");
        b.Append("</body></html>");

        return b.ToString();
    }

    Response RefClick(string oid) {
        var b = new StringBuilder();
        b.Append("<html><body><ul>");

        var o = DbHelper.FromID(ulong.Parse(oid));
        if (o != null) {
            reader.AllValues(o, (prop, name, value) => {
                b.AppendFormat("<li>{0}={1}</li>", name, value);
            });
        }

        b.Append("</ul></body></html>");

        return new Response() { Body = b.ToString(), StatusCode = 200 };
    }
}

class HtmlFormatter : ValueFormatter {

    public override string GetBoolean(bool? b) {
        var value = base.GetBoolean(b);
        if (b.HasValue) {
            if (b.Value) {
                value = string.Format("<b style=\"color: green\">{0}</b>", value);
            } else {
                value = string.Format("<b style=\"color: red\">{0}</b>", value);
            }
        }
        return value;
    }

    public override string GetObject(IObjectView o) {
        if (o == null) {
            return base.GetObject(o);
        }
        
        var url = string.Format("http://localhost:8080/object/{0}", o.GetObjectNo());
        return string.Format("<a href=\"{0}\">{1}</a>", url, o.GetObjectNo());
    }
}