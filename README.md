Azure Functions that get trigger when a Private Endpoint is created, and configures custom Private DNS

```
traces
| extend asd = parse_json(message)
| extend opname = asd.operationName
| where opname == "Microsoft.Network/privateEndpoints/write"
| project opname, asd.resourceUri, asd.status
```
