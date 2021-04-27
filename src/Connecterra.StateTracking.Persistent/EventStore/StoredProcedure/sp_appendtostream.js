function appendToStream(streamId, expectedVersion, events) {
    const isSuccessful = __.queryDocuments(__.getSelfLink(), {
        'query': 'SELECT Max(e.stream.version) FROM events e WHERE e.stream.id = @streamId',
        'parameters': [{ 'name': '@streamId', 'value': streamId }]
    }, function (error, items, options) {
        if (error) throw new Error("Fail to get expected stream version:" + error.message);
        if (!items || !items.length) throw new Error("No record found for the expected version.");

        var currentVersion = items[0].$1;
        if ((!currentVersion && expectedVersion == 0) || (currentVersion == expectedVersion)) {
            JSON.parse(events).forEach(event => __.createDocument(__.getSelfLink(), event));
            __.response.setBody(true);
        }
        else __.response.setBody(false);
    });

    if (!isSuccessful) throw new Error('Fail to execute the query.');
}