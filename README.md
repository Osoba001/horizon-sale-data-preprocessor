Horizon PreProcessor API
Overview

A .NET 8 Web API for pre-processing high-volume customer sales JSON arrays. Transforms each record into a standard format for the Horizon platform, including customer, product, and shipping details.

Key Features

Streaming JSON input to handle large datasets efficiently.

Resilient & robust error handling; invalid records are logged but processing continues.

POST /api/SaleDataTransform endpoint for submitting JSON arrays.


USAGE

HTTP File: Use HorizonPreProcessor.Api.http in the project.

Command Line (cURL):

curl -X POST https://localhost:7085/api/SaleDataTransform \
     -H "Content-Type: application/json" \
     -d @path/to/sales2.json


Output: JSON with success, summary, errors, and transformed data.
