const KNOWN_WEB_CACHE = "known-web-cache";

self.addEventListener("activate", function (event) {
    console.log("ServiceWorker activated.");
});

self.addEventListener("install", function (event) {
    event.waitUntil(
        caches.open(KNOWN_WEB_CACHE).then(function (cache) {
            return cache.addAll([]);
        }),
    );
});

self.addEventListener("fetch", (e) => { });