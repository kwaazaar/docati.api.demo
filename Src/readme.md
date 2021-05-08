Thank you for your interest in Docati!
--------------------------------------

# Overview
A sample project which demonstrates the use of Docati.Api can be found here: https://github.com/kwaazaar/docati.api.demo

# Docker/Linux
A Dockerfile is included to demonstrate how to use Docati.Api (using Docker) on Linux:
- Linux does not come with the fonts you find on Windows (with Office installed), so a few free fonts from MS are added
- GDI+ support is added for supporting image processing (can be omitted if you don't use ImageOf or BarcodeOf).

To build: docker build -t docatidemo -f Src/Dockerfile .

To run (Linux/WSL): docker run -rm -v $PWD:/out docatidemo /out
- The generated template will be put in your current folder

Check our site for more info on Docati: https://www.docati.com
And if you run into trouble, don't hesitate to ask us for help: support@docati.com
