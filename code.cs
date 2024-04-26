public class script: ScriptBase
{
public override async Task < HttpResponseMessage > ExecuteAsync()
{
    string operationId = (this.Context.OperationId).ToLower();
    // Check which operation ID was used
    switch (operationId) 
    {
        case "getfilecontent":
            return await this.GetFileContentOperation().ConfigureAwait(false);
            break;
        case "getmessages":
            return await this.GetMessagesOperation().ConfigureAwait(false);
            break;
        case "getassistants":
            return await this.GetAssistantsOperation().ConfigureAwait(false);
            break;
    }

    // Handle an invalid operation ID
    HttpResponseMessage response = new HttpResponseMessage(
        HttpStatusCode.BadRequest
    );
    response.Content = CreateJsonContent(
        $"Unknown operation ID '{this.Context.OperationId}'"
    );
    return response;
}

private async Task < HttpResponseMessage > GetFileContentOperation()
{
    // Use the context to forward/send an HTTP request
    HttpResponseMessage response = await this.Context.SendAsync(
        this.Context.Request,
        this.CancellationToken
    ).ConfigureAwait(continueOnCapturedContext: false);

    // Do the transformation if the response was successful
    if (response.IsSuccessStatusCode)
    {
        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(
        continueOnCapturedContext: false
        );
		
        // Convert stream to memorystream
        var memoryStream = new MemoryStream();
        responseStream.CopyTo(memoryStream);
        
        // Convert memory stream to byte array
        byte[] responseBytes = memoryStream.ToArray();

        // Convert byte array to base64
		string base64 = Convert.ToBase64String(responseBytes);

        // Create a JSON obejct for the response
        var newResult = new JObject
        {
            ["fileContent"] = base64,
        };
        response.Content = CreateJsonContent(newResult.ToString());
    }
     return response;
}
private async Task < HttpResponseMessage > GetMessagesOperation()
{
    // Use the context to forward/send an HTTP request
    HttpResponseMessage response = await this.Context.SendAsync(
        this.Context.Request,
        this.CancellationToken
    ).ConfigureAwait(continueOnCapturedContext: false);

    // Do the transformation if the response was successful
    if (response.IsSuccessStatusCode)
    {
        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(
        continueOnCapturedContext: false
        );

        //convert result to c# object
        var jsonObject = JObject.Parse(responseString);
        
        var newJson = new JObject
        // Rename "data" property to "messages"
        {
            ["object"] = jsonObject["object"],
            ["messages"] = jsonObject["data"],
            ["first_id"] = jsonObject["first_id"],
            ["last_id"] = jsonObject["last_id"],
            ["has_more"] = jsonObject["has_more"]
        };

        // Set the content of the response with the modified JSON
        response.Content = new StringContent(newJson.ToString());
    
        
    }
     return response;
}
private async Task < HttpResponseMessage > GetAssistantsOperation()
{
    // Use the context to forward/send an HTTP request
    HttpResponseMessage response = await this.Context.SendAsync(
        this.Context.Request,
        this.CancellationToken
    ).ConfigureAwait(continueOnCapturedContext: false);

    // Do the transformation if the response was successful
    if (response.IsSuccessStatusCode)
    {
        var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(
        continueOnCapturedContext: false
        );

        //convert result to c# object
        var jsonObject = JObject.Parse(responseString);
        
        var newJson = new JObject
        // Rename "data" property to "assistants"
        {
            ["object"] = jsonObject["object"],
            ["assistants"] = jsonObject["data"],
            ["first_id"] = jsonObject["first_id"],
            ["last_id"] = jsonObject["last_id"],
            ["has_more"] = jsonObject["has_more"]
        };

        // Set the content of the response with the modified JSON
        response.Content = new StringContent(newJson.ToString());
    
        
    }
     return response;
}

}
