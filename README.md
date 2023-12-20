# ImageProcessingGUI

[![MIT License][license-shield]][license-url]

## Overview

**ImageProcessingGUI** is an image processing application built with [AvaloniaUI][avalonia-ui-url] using F#.

## Features

- **Open File:** Choose a file from a file dialog.

- **Apply transformations:** Choose a transformation to apply. Application stays responsive.

- **Reset:** Easily reset to the original image.

- **Save File:** Choose a directory to save your file to.

## Table of contents

- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)

[//]: # (- [Usage]&#40;#usage&#41;)
[//]: # (    - [`update`]&#40;#depinspect-update&#41;)
[//]: # (    - [`diff`]&#40;#depinspect-diff&#41;)
[//]: # (    - [`list-all`]&#40;#depinspect-list-all&#41;)
[//]: # (    - [`find-divergent`]&#40;#depinspect-find-divergent&#41;)

[//]: # (- [Examples]&#40;#examples&#41;)
- [Licenses](#licenses)

## Getting Started

### Prerequisites

- [dotnet SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) 7.0 or higher
- OpenCL-compatible device with respective driver installed to utilize image processing on GPU

### Installation

Open the terminal and follow these steps:

1. Clone the repository:

    ```sh
    git clone git@github.com:artem-burashnikov/ImageProcessingGUI.git
    ```

2. Navigate to the project root:

    ```sh
    cd ImageProcessingGUI
    ```

3. Download ImageProcessing library to a local directory (the following command downloads it to the project root, assuming you did the previous step):

    ```sh
    wget https://github.com/artem-burashnikov/ImageProcessing/releases/download/v1.0.0/ImageProcessing.ArtemBurashnikov.1.0.0.nupkg
    ```

4. Add the project root to local Nuget package source:

   **Note**: Running the command will modify your `NuGet.config` file. **Execute from within a project root**.

    ```sh
    dotnet nuget add source $(pwd)
    ```

5. Now it is possible to restore tools and build a project:

    ```sh
    dotnet restore;
    dotnet build
    ```

## Licenses

The project is licensed under a [MIT License][license-url].

<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[license-shield]: https://img.shields.io/github/license/artem-burashnikov/ImageProcessingGUI.svg?style=for-the-badge&color=blue
[license-url]: LICENSE