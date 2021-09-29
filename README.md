# CameraMultiViewer
Hi, I made this app for viewing multiple cameras I have set up on an SMT Assembly (pick and place) Machine. But others may find it useful as well.

It can be used for eye tracking, please view the video below.

It uses the AForge.net and EmguCV's openCV wrapper for obtaining the USB video streams and conducting machine vision processing. Cuda is used when available.

[![Overview](http://img.youtube.com/vi/Mp8Z6vDXkm8/0.jpg)](http://www.youtube.com/watch?v=Mp8Z6vDXkm8 "Camera Viewer")


Build Instructions:
Requirements: 
a pc running windows 10 64bit edition with an Nvidia CUDA compatable g-card and at least two USB ports

Build Instructions:
All nuget packages are uptodate, so building is as easy! 

1. Download the latest free community edition of visual studio.
2. Download this repository via zip or git interface
3. Disable Common language exceptions (there are occasional exceptions but they are handled)

4: Click Start!

Notes:
It is highly advised that you save any work you have open when running this program. Running two usb cameras from the same host controller can cause instability. If you have multiple host controllers, ie a usb 2.0 and usb 3.0, you should plug one camera into the 2.0 and one into the 3.0 for best results.

Additionally the cameras used must support the MJPEG format. This information can normally be obtained before purchasing the camera. The c270 is generally adequate for this style of eye tracking and it supports MJPEG. 

Running a graphically intense program while running this program is more likely to result in instability.
