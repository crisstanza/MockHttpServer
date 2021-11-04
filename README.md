Mock HTTP Server
================

# Usage:

    ./MockHttpServer.exe -port <PORT> -pongPath <CUSTOM PATH>

## Examples:

    curl -v --request POST 'http://localhost:8910' --data-raw '{ "id": 1 }'
    curl -v --request POST --header 'Accept: application/json' 'http://localhost:8910/api/xyz' --data-raw '{ "id": 2 }'
    curl -v --request GET 'http://localhost:8910/API/XYZ?ping=test.html'
    curl -v --request GET 'http://localhost:8910/API/XYZ?ping=test.txt'
    curl -v --request PUT 'http://localhost:8910/API/XYZ?ping=test.json'

### Links:
    https://favicon.io/favicon-converter/
