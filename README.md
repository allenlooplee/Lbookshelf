Introduction
==========

L'Bookshelf is an app that help you import and manage your ebooks in local or removable drive.

Features
==========

- Import ebooks (currently pdf only) to a specified location, local or removable.
- Read metadata from pdf with [PDFSharp](http://pdfsharp.com/PDFsharp/).
- Fetch all required metadata such as title, authors, publisher, description, etc with [Google Books API](https://developers.google.com/books/) or [Douban API](http://developers.douban.com/wiki/?title=guide).
- Manage ebooks through different dimensions such as category, publisher, or booklist. Under the hook, ebooks are stored in a directory structure based on categories.
- Search ebooks through their title.
- Pin frequently opened ebooks to home page.
- Cache pinned books in app folder. This is useful when the user stores the books in a removable drive that's not always at hand.

Build and run
==========

To run L'Bookshelf on Windows, you'll need to build it with Visual Studio 2013 or [Visual Studio Express 2013](http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200395106). .NET Framework 4.5 is required on the client machine. The source code is licensed under MIT License.
