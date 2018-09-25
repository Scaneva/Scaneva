# Scaneva
Software for controlling an SECM (scanning electrochemical microscope)

Scaneva is a Software Package (for Windows) written in .Net C# that was originally developed to control an SECM instument. However, due to the modular design of Hardware and Experiment Blocks it can be easily adapted to automate almost any Lab Experiment/ Measurements.

#### Getting started

1. Scaneva is developed with Visual Studio. Since the solution files provided are for Visual Studio we recommend using it for a quick start. 
2. If you want to use our GUI get the required OxyPlot packages from [nuget.org](https://www.nuget.org/packages?q=oxyplot)
3. Clone/ download the repo
4. You should be good to go

#### Supported Hardware and Dependencies

At the moment we implemented Hardware support for the following Vendors/ Devices:

Vendor | Hardware | Dependencies
--------|---------|-------------
[PalmSens BV](https://www.palmsens.com) | PalmSens3, PalmSens4, EmStat | PalmSens.Core.dll 5.4.0223, PalmSens.Core.Windows.dll 5.4.0223
[LANG GmbH & Co. KG](http://www.lang.de) | LStep PCIe |
