# URL Shortener API

A simple URL shortening service built with .NET Core 8 and PostgreSQL.

### Prerequisites
- Docker
- Docker Compose

### Running the Application
1. Clone the repository
2. Navigate to the project directory
3. Run: docker compose up

The API will be available at `http://localhost:8080`  
Swagger UI will be available at `http://localhost:8080/swagger/index.html`

### 1. Shorten URL
Converts a long URL into a shortened version.

**Endpoint:** `POST /UrlShortener/shorten`

**Parameters:**
- `originalUrl` (query parameter, required): The URL to be shortened
  - Cannot be empty

**Request Example:**
curl -X 'POST' \
  'http://localhost:8080/UrlShortener/shorten?originalUrl=https%3A%2F%2Fwww.youtube.com' \
  -H 'accept: */*' \
  -d ''

**Response**
json
{
"shortenedUrl": "ab12cd34"
}

### 2. Expand URL
Retrieves the original URL from a shortened URL.

**Endpoint:** `GET /UrlShortener/expand/{shortenedUrl}`

**Parameters:**
- `shortenedUrl` (path parameter, required): The 8-character shortened URL code

**Request Example:**
curl -X 'GET' \
  'http://localhost:8080/UrlShortener/expand/3cde63f8' \
  -H 'accept: */*'

**Response**
json
{
"originalUrl": "https://youtube.com"
}

### 3. Redirect to Original URL
Redirects to the original URL directly.

**Endpoint:** `GET /UrlShortener/redirect/{shortenedUrl}`

**Parameters:**
- `shortenedUrl` (path parameter, required): The 8-character shortened URL code

**Usage:**
- Open in browser: `http://localhost:8080/UrlShortener/redirect/ab12cd34`
- Or use curl: ` curl -L -X 'GET' \
'http://localhost:8080/UrlShortener/redirect/ab12cd34' `

### Checking the database data
# Connect to database in docker terminal 
psql -U postgres -d urlshortener

# Useful commands:
\dt                   # List tables
\d "UrlMappings"     # Describe table
SELECT * FROM "UrlMappings";  # View all records
