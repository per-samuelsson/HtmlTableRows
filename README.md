# What?
Allow you to do this

`star \HtmlTableRows\Program.cs`

and then enjoy this

```C#
Handle.GET("/books", () => {
  return Db.SQL("SELECT b FROM Book b");
});
```

to generate this

Author | Book
-------|-----
Hermann Hesse | Siddhartha
Ken Kesey | One flew over the cuckoo's nest

#How?
By using [Starcounter](http://starcounter.io) extension points like this:

```C#
Main() {
  var services = Application.Current.Host.GetServiceContainer();
  services.Register<IQueryRowsResponse>(new Program());
}

public Response Respond<T>(QueryResultRows<T> rows) {
  // Generate a table from the result...
}
```

See [Program.cs](https://github.com/per-samuelsson/HtmlTableRows/blob/master/src/HtmlTableRows/Program.cs) for the full detail.
