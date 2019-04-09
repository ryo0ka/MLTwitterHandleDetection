# MLTwitterHandleDetection
Twitter username detection sample for Magic Leap.

Using Google Cloud Vision API, the demo app will recognize texts that the AR glasses are seeing & extract Twitter handles.

For now with the current ML1 device and reasonable WiFi connection, 

1. Camera resolution is not sufficient for the best UX. Currently user will have to hold the text source (PC display or smartphone) very close to the AR glasses in order to recognize texts sufficiently. This can be resolved "over time" as AR glasses improve hardware-wise. Though, that could contribute to another bottleneck observed:
2. Network latency or/and Cloud API's response speed is not sufficient for the best UX. With a 200mbps WiFi connection, Google Cloud Vision's REST API call takes ~2.5 secs per image of 960x1080 pixels.

Overall this feature is experimental for the current hardware situation and probably not ready for production yet.
