# zendesk-marketplace
A coding challenge for Zendesk interview

## Possible improvements:
* Use Property expression instead of reflection to improve property listing speed
* Handle DateTimeOffset differently in LiteDb as they are converted and displayed as UTC dates
* In production I would rather use ElasticSearch instead of LiteDb as it supports indexing out of the box and can keep all the data formats required. It requires a separate server, that's why I haven't used it

## Notes
I didn't write tests for database-related classes as the majority of logic is happening inside the LiteDb engine, which is already tested
