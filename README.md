# MLTwitterHandleDetection
Twitter username detection sample for Magic Leap in Unity.

Using Google Cloud Vision API, the demo will recognize Twitter handles that user is seeing.

Unresolved challenges in this demo: 

1. Camera resolution is not sufficiently large. User has to get very close to the text source (PC display or smartphone) in order for Google Vision to recognize texts on the display stably.
2. Network latency or/and Cloud API's response speed is not sufficiently high in order to upload/process high-res images. One call overall takes ~2.5 secs per 960x1080 pixels on a stable 200mbps WiFi connection.

Some intermediate features that might be useful for you:

1. Google Vision client for text recognition (`GVTextClient.cs`), hiccup-free, probably usable in any platforms (except WebGL), built on top of UniRx async/await framework for Unity 2018 and later.
2. Async readback of Magic Leap camera feed (`MLAsyncCaptureDevice.cs`). The class uses Magic Leap's "camera preview" feature and a compute shader (`Demo/Resources/AsyncRead.compute`) to extract the camera's frames without causing a hiccup on UI thread. This approach can produce an image per ~5 frames, which is a good enough latency for cloud-based image recognition. This approach is more convenient than the "official" approach (`MLCaptureDevice.cs`) because it gives you a raw image instead of a JPEG, so that you can optimize the image (e.g. reduce dimension) for uploading to cloud.

Feel free to reuse any part of the project under MIT license and ask any questions. Remember to hit Star above, and please Like/RT the demo on Twitter: https://twitter.com/ryoichirooka/status/1115499972294656000
