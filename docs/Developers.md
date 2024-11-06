# Chameleon Developer Guide

This guide provides information for developers who want to contribute to the Chameleon project. It includes setup instructions, coding standards, and other useful information to help you get started.

## Table of Contents
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup](#setup)
- [Development](#development)
  - [Project Structure](#project-structure)
  - [Coding Standards](#coding-standards)
  - [Building the Project](#building-the-project)
  - [Running Tests](#running-tests)
- [Contributing](#contributing)
  - [Pull Request Guidelines](#pull-request-guidelines)
  - [Code of Conduct](#code-of-conduct)
- [Resources](#resources)

## Getting Started

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or later
- Git

### Setup
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/chameleon.git cd chameleon

2. **Open the Project in Visual Studio**:
   - Open Visual Studio.
   - Select `File > Open > Project/Solution`.
   - Navigate to the cloned repository and open the `Chameleon.sln` file.

3. **Restore Dependencies**:
   - In Visual Studio, open the Package Manager Console and run:
   ```bash 
	dotnet restore

## Development

### Project Structure
- `Chameleon.lib`: Core logic and business rules.
- `Chameleon.app.Avalonia`: User interface components.
- `Chameleon.lib.Tests`: Unit and integration tests.

### Coding Standards
- Follow the [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Use meaningful variable and method names.
- Write XML documentation for public methods and classes.

### Building the Project
- To build the project, use the following command in the terminal:
   ```bash 
	dotnet build

### Running Tests
- To run all tests, use the following command:
  ```bash
  dotnet test

## Contributing

### Pull Request Guidelines
- Fork the repository and create your branch from `main`.
- Ensure your code follows the coding standards.
- Write or update tests as necessary.
- Ensure all tests pass before submitting a pull request.
- Provide a clear description of your changes in the pull request.

### Code of Conduct
- Be respectful and considerate of others.
- Follow the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/version/2/0/code_of_conduct/).

## Resources
- [Figma Design](https://www.figma.com/design/XlKce35jHYGiGlz6hBPcsk/Chameleon-Windows-app-(Copy)-2024?node-id=0-1&t=Uc8tbCol2IfLlLTd-0)
- [Avalonia UI](https://github.com/AvaloniaUI/Avalonia)
- [FluentAvalonia Theme](https://github.com/amwx/FluentAvalonia)
- [WinUI Gallery](https://www.microsoft.com/store/productId/9P3JFPWWDZRC?ocid=pdpshare)
- [MVVM Toolkit](https://www.microsoft.com/store/productId/9NKLCF1LVZ5H?ocid=pdpshare)
- [Community Toolkit](https://www.microsoft.com/store/productId/9NBLGGH4TLCQ?ocid=pdpshare)

Thank you for contributing to Chameleon! Your efforts help make this project better for everyone.
