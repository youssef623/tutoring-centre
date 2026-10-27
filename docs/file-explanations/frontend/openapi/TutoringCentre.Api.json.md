# `frontend/openapi/TutoringCentre.Api.json`

## Purpose

JSON data with top-level keys `openapi`, `info`, `paths`, `components`, `tags`. Its consumers determine whether this is build configuration, a contract or a language resource.

## Where it fits

Category: **configuration**. [Open current file](../../../../frontend/openapi/TutoringCentre.Api.json). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 6020 lines and 147,397 bytes. Source hash: `20c231078e844d9bb8e67483d9ba4fee9371ecc1da2216ce00dd426f92296ced`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Read the data or directives in order; there are no recognized code declarations in this file.

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```json
{
  "openapi": "3.1.1",
  "info": {
    "title": "TutoringCentre.Api | v1",
    "version": "1.0.0"
  },
  "paths": {
    "/api/system/info": {
      "get": {
        "tags": [
          "PlatformEndpoints"
        ],
        "operationId": "GetSystemInfo",
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/SystemInfoDto"
                }
              }
            }
          },
          "500": {
            "description": "Internal Server Error",
            "content": {
              "application/problem+json": {
                "schema": {
                  "$ref": "#/components/schemas/ProblemDetails"
                }
              }
            }
          }
        }
      }
    },
    "/api/auth/antiforgery": {
      "get": {
        "tags": [
          "AuthEndpoints"
        ],
        "operationId": "GetAntiforgeryToken",
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/AntiforgeryTokenResponse"
                }
              }
            }
          }
        }
      }
    },
    "/api/auth/login": {
      "post": {
        "tags": [
          "AuthEndpoints"
        ],
        "operationId": "Login",
        "requestBody": {
          "content": {
            "application/json": {
              "schema": {
                "$ref": "#/components/schemas/LoginRequest"
              }
            }
          },
          "required": true
        },
        "responses": {
          "200": {
            "description": "OK",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/MeDto"
                }
              }
            }
          },
          "400": {
            "description": "Bad Request",
            "content": {
              "application/problem+json": {
                "schema": {
                  "$ref": "#/components/schemas/HttpValidationProblemDetails"
```

The excerpt stops at line 90; use the original file for the remaining 5930 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
