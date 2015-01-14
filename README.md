# Document Storage

When taking out a loan a customer may need to provide several items of data to substantiate their application – an example could be a scan of a driving license, or a utility bill.

These documents, together with any others that may be produced internally can be stored together as a unit and recalled at will. A pictorial representation of an application is shown below…

There is no inherent structure here, it’s basically a simple collection of files as I don’t see the need to have a large unwieldy structure (i.e. set of directories) for an application. If we did want to segregate files we might well break these down into two containers, one accessible by the end user and another that is internal to Oakbrook. The files are still associated with one application, but we might choose to split these into two separate containers. That makes security much easier too, as we can create one container just for user accessible data and lock the other down completely.
## The API
We need a mechanism where we can store and retrieve files, so I came up with the simplest interface I could think of for this…
>public interface IFileContainer
>{
>  Stream GetFile(string filename);
>  void StoreFile(string filename, Stream fileContent, string contentType);
>  IEnumerable<string> GetFilenames();
>}
