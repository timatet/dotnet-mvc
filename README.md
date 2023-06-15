[![Docker Image CI](https://github.com/timatet/dotnet-mvc/actions/workflows/docker-image.yml/badge.svg)](https://github.com/timatet/dotnet-mvc/actions/workflows/docker-image.yml)

# Dotnet MVC Project

## Description
The first version of the web store.

The theme of the store: **tourist equipment** \
Version: **23.21.1**

### Completed goals
- Product catalog
- Catalog filters
- Detailed product card
- Shopping cart with the ability to add
- Registration/authorization/authentication
- Separation of roles: client/administrator
- Creating/editing/deleting products for the administrator
- Error/Info Pages

### Goals for the next version
- Administrator's personal account to manage users and roles
- Personal Account page

## Technology Stack
- Net5.0
- NPM
- Bootstrap 5.2.3

## Known Bugs
- **Problem:** Can cause an error when trying to checkout if no item is selected. \
  **Required solution:** add a processing that sends a notification about the impossibility of such an action
  
EOF
