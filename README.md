# Document Storage

When taking out a loan a customer may need to provide several items of data to substantiate their application – an example could be a scan of a driving license, or a utility bill.

These documents, together with any others that may be produced internally can be stored together as a unit and recalled at will. A pictorial representation of an application is shown below…

![File Storage](https://raw.githubusercontent.com/morganski/AzFileStore/master/DS01.png)

There is no inherent structure here, it’s basically a simple collection of files as I don’t see the need to have a large unwieldy structure (i.e. set of directories) for an application. If we did want to segregate files we might well break these down into two containers, one accessible by the end user and another that is internal to us. The files are still associated with one application, but we might choose to split these into two separate containers. That makes security much easier too, as we can create one container just for user accessible data and lock the other down completely.
## The API
We need a mechanism where we can store and retrieve files, so I came up with the simplest interface I could think of for this…

    public interface IFileContainer
    {
        Stream GetFile(string filename);
        void StoreFile(string filename, Stream fileContent, string contentType);
        IEnumerable<string> GetFilenames();
    }

As we’re dealing with files I’ve made the decision that a file has a filename which isn’t too surprising. We can list all files (from a container), get a specific file from that container, and store a new file (which will replace an existing one) too. This is a minimal API – I’ve not added any metadata storage options at the moment as I’m trying to keep this interface simple. Until we know what metadata (if any) we need, there’s not much point dealing with it.
You might wonder about the absence of a “delete file” method. I pondered this for a while and decided that this was a much less common event, so if we do permit a single file to be deleted I’d suggest that we create a distinct interface for that operation only.
I think it’s more likely that we’ll need to keep everything for a period of time, and then archive/delete an entire container’s worth of files after whatever retention period we need to adhere to.
## External Access
In addition to internal access through the file container API it would also be advantageous to permit external access to a file so that we could list accessible files in a portal and allow a user to view these files.
To access a file we need a URI, and this URI should ideally expire after a period of time, so that a user’s sensitive documents couldn’t inadvertently be accessed by another user.
I’ve created a second interface (derived from IFileContainer) which adds on a method that returns a URI for a file as follows…

    public interface ISecureFileContainer : IFileContainer
    {
        Uri GetSecureFileUri(string filename);
    }

I’ve deliberately included the word “secure” here to indicate to the user that the Uri provided has some security aspect to it. Whether this needs to be derived from IFileContainer is debatable, I chose to do it as it’s likely that we may retrieve a list of files, display these to the user, and provide URI links to these files so that when the user clicks a filename they’ll be able to view that file directly in the browser (assuming that a viewer for the file type exists).
## Azure Implementation
I have created an implementation of both interfaces using Windows Azure which can be used as an exemplar. We could also implement one using S3 and, should we wish to, one using a file system – however using the latter we’d also need to write the security aspects of it too, which Azure already does for us.
The AzureFileContainer class contains a constructor as follows…

    public AzureFileContainer(string connectionString, string containerName, TimeSpan fileShareDuration)

The connection string contains the details used to connect to Windows Azure, and the container name would be a unique string – I’d suggest it was the application Id we use in our other systems. Lastly the fileShareDuration defines how long a link will be valid.
We would expose this container in Autofac as a dependency so that it could be imported as follows…

    Func<string, ISecureFileContainer>

Here the string would be the name of the storage container.
The AzureFileContainer implements the GetSecureFileUri as shown below…

    public Uri GetSecureFileUri(string filename)
    {
        var blockBlob = _container.GetBlockBlobReference(filename);

        var constraints = new SharedAccessBlobPolicy
        {
            SharedAccessExpiryTime = DateTime.UtcNow.Add(_fileShareDuration),
            Permissions = SharedAccessBlobPermissions.Read
        };

        var token = blockBlob.GetSharedAccessSignature(constraints);

        return new Uri(blockBlob.Uri, token);
    }


This method retrieves the reference to the file and then creates a security policy that provides time limited access to the file via a dependency (defined as a timespan). The form of URI returned is shown below…

    http://127.0.0.1:10000/devstoreaccount1/sausage/jim.htm?sv=2014-02-14&sr=b&sig=9gBjxDk4JWweZgohNCZMXj3f34JF4pgT8lS%2FtIRR1KI%3D&se=2015-01-14T14%3A26%3A09Z&sp=r

Here I’m using a local Azure storage account – a remote account URI is as follows…

    https://somethingorother.blob.core.windows.net/sausage/jim.htm?sv=2014-02-14&sr=b&sig=mW1WxAghZ5CfsSbNF7M3E0eTNfMLXLkt2K0qg180Jk8%3D&se=2015-01-14T14%3A34%3A52Z&sp=r

Note that both the container name (in this case sausage) and the filename (jim.htm) are exposed to the caller. If a request is made to retrieve the container (i.e. by removing jim.htm above and all subsequent URL parameters) then Azure will respond with an error…

    <Error>
      <Code>ResourceNotFound</Code>
      <Message>The specified resource does not exist. 
        RequestId:f006b315-0001-004c-3b79-48872b000000 
        Time:2015-01-14T14:38:04.4664313Z
      </Message>
    </Error>

We could expose further methods to the caller by altering security on the container and blobs but the default is to lock down access to everything.

## Auditing Access

Whilst we cannot verify that a user read a document, we could audit all interactions with a document so as to provide an audit trail of who has downloaded what. By wrapping the file container service in an AuditedFileContainerService we could then record all file accesses, whether read or write. We’d use Thread.CurrentPrincipal to identify the user, and use this to record access. In addition to the user it would also be worthwhile logging a timestamp which could be used to order the file access results when displayed to a user.
This would no doubt be a worthwhile addition from a regulatory standpoint, as this would record exactly who had been granted access to a specific file.









