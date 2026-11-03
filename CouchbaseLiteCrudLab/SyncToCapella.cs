using Couchbase.Lite;
using Couchbase.Lite.Sync;

namespace CouchbaseLiteCrudLab;

public static class SyncToCapella
{
    public static async Task RunOnceAsync(
        Database database,
        Collection collection,
        string publicConnectionUrl,
        string username,
        string password)
    {
        var target = new URLEndpoint(new Uri(publicConnectionUrl));
        var config = new ReplicatorConfiguration(target);

        config.AddCollection(collection);
        config.ReplicatorType = ReplicatorType.PushAndPull;
        config.Continuous = false;
        config.Authenticator =
            new BasicAuthenticator(username, password);

        using var replicator = new Replicator(config);

        var listenerToken = replicator.AddChangeListener((sender, change) =>
        {
            Console.WriteLine(
                $"Replication state: {change.Status.Activity}");

            Console.WriteLine(
                $"Progress: {change.Status.Progress.Completed}/" +
                $"{change.Status.Progress.Total}");

            if (change.Status.Error is not null)
                Console.WriteLine(
                    $"Replication error: {change.Status.Error}");
        });

        replicator.Start();

        while (replicator.Status.Activity != ReplicatorActivityLevel.Stopped)
        {
            await Task.Delay(500);
        }

        replicator.RemoveChangeListener(listenerToken);

        if (replicator.Status.Error is not null)
            throw new InvalidOperationException(
                "Replication failed: " + replicator.Status.Error);

        Console.WriteLine("Replication completed.");
    }
}
