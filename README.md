# Overview
Docati.Api is a library for generating Word or PDF documents and is available as a [nuget package](https://www.nuget.org/packages/Docati.Api)
This repository contains a sample project which demonstrates the (ease of) use of Docati.Api.

# Docker/Linux
A Dockerfile is included to demonstrate how to use Docati.Api (using Docker) on Linux:
- Linux does not come with the fonts you find on Windows (with Office installed), so a few free fonts from MS are added
- GDI+ support is added for supporting image processing (can be omitted if you don't use ImageOf or BarcodeOf).

## Building
From the repo root run: docker build -t docatidemo -f Src/Dockerfile .

## Running
To run from Linux (or WSL): docker run -rm -v $PWD:/out docatidemo /out
- The generated template will be put in your current folder

## Running ready-made image
The image is also available from Docker Hub: [kwaazaar/docatidemo](https://hub.docker.com/repository/docker/kwaazaar/docatidemo)
To run: docker run -v $PWD:/out kwaazaar/docatidemo /out

Check our site for more info on Docati: https://www.docati.com
And if you run into trouble, don't hesitate to ask us for help: support@docati.com
