# Header Propagation Repro

I've tried like heck to get this all to work in a devcontainer, if you prefer to work that way.

- Make a GET request with an ETag: say, `curl --include --url 'http://localhost:8080/widgets/1' --header "etag: W/'1'"`.
- You'll get an exception (because the remote URL is invalid), but note that the request was attempted.
- Comment out the direct call to `_innerClient.GetWidgetAsync` in `CachingWidgetClient`.
- Repeat the request.
- Note the exception. (Matching the one from the ticket, I hope!)
