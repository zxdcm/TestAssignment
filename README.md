# Test Assignment

## Introduction

This README provides an overview of the requirements, the chosen implementation strategies for these requirements, and what needs to be implemented to deliver it to production

## Disclaimer

Due to the time constraint recommendation (6 hours) for this assignment, querying of posts and implementation of the change feed processor are not implemented.

## Services Overview

### FeedAPI

The FeedAPI is the core service responsible for CRUD over posts and comments.

### BlobImageFunctions (BlobImageConverter)

The `BlobImageFunctions` project contains the `BlobImageConverter` function, which does the following:
- **Image Conversion**: Automatically converts uploaded images to .jpg.
- **Image Resizing**: Resizes images to 600x600

### FileUploadAPI

The FileUploadAPI is a dedicated service to upload large image files in chunks to blob storage.

## Usage Forecast Calculations

Posts container document estimated size:
- **IDs (Guid)**: 4 * 36 bytes = 144 bytes
- **DateTime**: 20 bytes
- **String (Average)**: 200 bytes
- **Comments Count**: 10 bytes
- **Comments** (2 comments per document): 2 * (36 (ID) + 200 (Content)) = 472 bytes
- **JSON Overhead**: Approximately 100 bytes
- **Cosmos DB Overhead (Estimated)**: 200 bytes

**Total**: 144 + 20 + 200 + 10 + 472 + 100 + 200 = 1146 bytes => rounded to 2000 bytes

1000 uploaded images per hour => 1000 post documents per hour; 
24,000 posts per day => 8,760,000 per year => 16.3 GB per year for posts container

## Functional Requirements

#### onvert uploaded images to .jpg format and resize to 600x600
- **Implementation Strategy**: 
  Conversion should be done in the background using an Azure Event Grid listener on the source blob.

#### Serve images only in .jpg format
- **Implementation Strategy**: 
  The Feed API returns posts with URLs pointing to a CDN, which in turn points to the container with the converted blob.

## Non-Functional Requirements Implementation Notes

### Maximum response time for any API call except uploading image files - 50 ms

#### Create Posts, Create Comments, Delete Comments 
- **Implementation Strategy**: 
  Cosmos DB (NoSQL) is used as the main storage, where write calls take <= 15ms at the 99th percentile.
  Posts and comments are stored in separate containers.
  Posts container is partitioned by creatorId
  Comments container is partitioned by postId

#### Posts should be sorted by the number of comments (desc)
- **Implementation Strategy**: 
  Cosmos DB query against posts container/dedicated container, partitioned by another key to avoid cross-partition queries. Given the usage forecast, it will take 3 years to fill up a physical partition (50GB) with posts, so cross-partition queries should not be a problem; however, this needs to be verified. There should be a change feed listener for the comments container that updates the posts' comments property and comments count. Later on, one of the following can be done:
    - Query against posts ordered by comments count and cache the result.
    - A background worker runs periodically and maintains structure (ordered IDs of posts) & hot posts in Redis cache.

### Users have a slow and unstable internet connection

#### Maximum image size - 100MB
- **Implementation Strategy**:
  File upload should be done in chunks using a dedicated api that handles uploads in chunks (e.g., 2 MB).

  An alternative solution is to issue a Shared Access Signature token (30-minute lifetime & IP address pinning) and let the client app upload directly to the blob.

## Delivering to Production

### Infrastructure Setup & Deployment

- CI/CD configuration.
- Kubernetes for Feed API & FileUpload API (docker files to build images, k8s configuration and etc).
- IaC configuration for all resources. It's done only partially refer to the `deploy/azure`

### Monitoring

Monitoring with Application Insights should be configured for all APIs and Azure Functions.

### Testing

- Unit and integration tests should be implemented.

### Authentication & Authorization 

Authentication should be implemented