<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Thanks again! Now go create something AMAZING! :D
***
***
***
*** To avoid retyping too much info. Do a search and replace for the following:
*** CreatePhotonW, mywarez, @CreatePhotonW, email, MyWarez, Malware Kill Chain build framework
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
<!--
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]
-->


<!-- PROJECT LOGO -->
<br />
<p align="center">
  <a href="https://github.com/CreatePhotonW/mywarez">
<!--    <img src="images/logo.png" alt="Logo" width="80" height="80"> -->
  </a>

  <h3 align="center">MyWarez</h3>

  <p align="center">
    Malware Kill Chain build framework
    <br />
<!--    <a href="https://github.com/CreatePhotonW/mywarez"><strong>Explore the docs »</strong></a> -->
    <br />
    <br />
    <!--
    <a href="https://github.com/CreatePhotonW/mywarez">View Demo</a>
    ·
    -->
    <a href="https://github.com/CreatePhotonW/mywarez/issues">Report Bug</a>
    ·
    <a href="https://github.com/CreatePhotonW/mywarez/issues">Request Feature</a>
  </p>
</p>



<!-- TABLE OF CONTENTS -->
<details open="open">
  <summary><h2 style="display: inline-block">Table of Contents</h2></summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

<!--
[![Product Name Screen Shot][product-screenshot]](https://example.com)
-->

MyWarez is a malware kill chain build framework that can be used to automate the build process of endpoint attack scenerios. With this framework, you can construct and modify scenerios such as the one below in an automated fashion. No more spending hours manaully compiling code to make a modifcation to 10 level deep compiled resource!
1. Hosts an Edge exploit on an HTTP Server (HTMLMTH)
2. Applies HTML, HTTP, TCP, IP evasions to the exploit response traffic.
3. Exploits the browser to execute a command line payload such as PowerShell.
4. Reflectively loads a next stage DLL containing an LPE
5. Exploits an LPE to take control over \windows\license.rtf
6. Overwrites \windows\license.rtf with a next stage DLL payload
7. Uses the Diaghub technique to load the DLL, license.rtf, to escalate privilege
8. Establishes persistence using the AppCertDLLs technique
9. Invokes Anti-Debugger techniques and only executes the next stage when the next stage DLL is loaded by winlogon.exe
10. Launches a Reverse HTTPS meterpreter shell over TCP:636


<!-- 
### Built With

* []()
* []()
* []()

-->



<!-- GETTING STARTED -->
## Getting Started

To get a local copy up and running follow these simple steps.

### Prerequisites

* C#.NET programming experience 
* Visual Studio
* .NET Core 3.1
* Visual C++ build tools
* Python3
* PyInstaller
* GO
* TDM-GCC
* Metasploit-Framework
* Windows Sub Linux


### Installation

0. Install dependencies and ensure the following are in the System Path
   ```
   go
   python
   pyinstaller
   msfvenom
   bash
   ```
1. Clone the repo recursively
   ```sh
   git clone https://github.com/CreatePhotonW/MyWarez.git --recursive
   ```
2. Open MyWarez.sln in Visual Studio
3. Clean Solution, Build Solution, Run the Examples project.


<!-- USAGE EXAMPLES -->
## Usage

_For code examples on the framework's usage, please refer to the [Examples project.](Examples)_

_Read through the commented examples in the following order:_
1. [OutputMechanism](Examples/AttackExamples/OutputMechanism.cs)
2. [NativeCode](Examples/AttackExamples/NativeCode.cs)
3. [Office](Examples/AttackExamples/Office.cs)
4. [Misc](Examples/AttackExamples/Misc.cs)


<!-- ROADMAP -->
## Roadmap

See the [open issues](https://github.com/CreatePhotonW/mywarez/issues) for a list of proposed features (and known issues).



<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to be learn, inspire, and create. Any contributions you make are **greatly appreciated**.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request



<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE` for more information.



<!-- CONTACT -->
## Contact

CreatePhotonW - [@CreatePhotonW](https://twitter.com/CreatePhotonW)

Project Link: [https://github.com/CreatePhotonW/mywarez](https://github.com/CreatePhotonW/mywarez)



<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/CreatePhotonW/repo.svg?style=for-the-badge
[contributors-url]: https://github.com/CreatePhotonW/repo/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/CreatePhotonW/repo.svg?style=for-the-badge
[forks-url]: https://github.com/CreatePhotonW/repo/network/members
[stars-shield]: https://img.shields.io/github/stars/CreatePhotonW/repo.svg?style=for-the-badge
[stars-url]: https://github.com/CreatePhotonW/repo/stargazers
[issues-shield]: https://img.shields.io/github/issues/CreatePhotonW/repo.svg?style=for-the-badge
[issues-url]: https://github.com/CreatePhotonW/repo/issues
[license-shield]: https://img.shields.io/github/license/CreatePhotonW/repo.svg?style=for-the-badge
[license-url]: https://github.com/CreatePhotonW/repo/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/CreatePhotonW
