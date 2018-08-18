# CameraMultiViewer
Hi, I made this app for viewing multiple cameras I have set up on an SMT Assembly (pick and place) Machine. But others may find it useful as well.

It can be used for eye tracking, please view the video below.

It uses the AForge.net and EmguCV's openCV wrapper for obtaining the USB video streams and conducting machine vision processing. Cuda is used when available.

[![Overview](http://img.youtube.com/vi/Mp8Z6vDXkm8/0.jpg)](http://www.youtube.com/watch?v=Mp8Z6vDXkm8 "Camera Viewer")

Todo: a lot of the code will be organized a lot better in the next few weeks, just wanted to get something out there as soon as possible. Eventually the eyetracking will be placed in it's own independent project that can be fed two bitmap images and return the eye tracking results. This will make it easier to use the code in other systems if people so desire.

I will post more detail instructions on how to get the project compiling, but with a fresh install of visual studio 2017 on a 64 pc with a Cuda compatible graphics card, you should be able to load the solution and the nuget packages will be updated automatically. 

Important: This program doesn't really play nice with other highly graphical intensive programs, such as Premiere pro. Disabling cuda rendering on Premiere pro seemed to fix the issue with me. It would be wise to keep extraneous graphics processing to a minimum while using this program, worst case your computer may crash.

Important as well, you may have noticed that most programs do not let you open two usb cameras simultaneously. There is a reason behind this, that likely has to due with the sub microsecond frame length of usb 2.0 protocol. If your computer has two seperate USB controllers (such as a usb 2.0 and a usb 3.0) I have found that pluging the cameras into separate controllers will increase your performance and decrease the possibility of dropped frames and program instability. Keeping usb extenders to a minimum will help as well. Above 20ft you will likely see issues. That being said, I've run 4 separate usb cameras on my computer with no issues.

Feel free to reach out to me with any inquiries, I will be monitoring this repo for some time.
