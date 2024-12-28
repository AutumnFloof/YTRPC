// ==UserScript==
// @name         YT Discord RPC
// @namespace    http://tampermonkey.net/
// @version      2024-12-28
// @description  Send Media Information To DOTNET Application for Discord Rich Presence
// @author       AutumnFloof
// @match        https://music.youtube.com/*
// @icon         https://www.google.com/s2/favicons?sz=64&domain=tampermonkey.net
// @grant        none
// ==/UserScript==
(function() {
    'use strict';

    let socket;
    let currentMetadata = null; // Variable to store the current song's metadata
    let retryTimeout = 1000; // Initial retry timeout
    let trackURL = null; // Variable to store music track URL or ID
    let bufferedMetadata = null; // To store data if WebSocket is not open
    let maxRetries = 10; // Limit to number of retries (for safety)
    let retryCount = 0; // Retry count tracker

    function connectWebSocket() {
        socket = new WebSocket("ws://localhost:5000/ws");

        socket.onopen = () => {
            console.log("Connected to WebSocket server!");
            retryCount = 0; // Reset retry count on successful connection
            retryTimeout = 1000; // Reset retry timeout

            // Send the most recent buffered metadata, if any
            if (bufferedMetadata) {
                console.log("Sending buffered data:", JSON.stringify(bufferedMetadata));
                socket.send(JSON.stringify(bufferedMetadata));
                bufferedMetadata = null; // Clear buffered data
            }

            // Also send the current metadata if we have it
            if (currentMetadata) {
                console.log("Sending current metadata:", JSON.stringify(currentMetadata));
                socket.send(JSON.stringify(currentMetadata));
            }

            checkMetadata(); // Start checking for metadata changes
        };

        socket.onerror = (error) => {
            console.error("WebSocket error:", error);
        };

        socket.onclose = () => {
            console.log("WebSocket connection closed. Reconnecting...");
            if (retryCount < maxRetries) {
                setTimeout(connectWebSocket, retryTimeout); // Reconnect after timeout
                retryTimeout = Math.min(retryTimeout * 2, 60000); // Exponential backoff, capped at 60 seconds
                retryCount++;
            } else {
                console.log("Maximum retries reached. Please check the WebSocket server.");
            }
        };
    }

    // Function to check and send metadata only if it has changed
    function checkMetadata() {
        setInterval(() => {
            if ('mediaSession' in navigator && navigator.mediaSession.metadata) {
                const metadata = navigator.mediaSession.metadata;
                const trackURL = window.location.href;
                const artwork = metadata.artwork;
                const data = {
                    title: metadata.title || "Unknown",
                    artist: metadata.artist || "Unknown",
                    album: metadata.album || "Unknown",
                    url: trackURL || "",
                    artwork: artwork.length > 0 ? artwork[0].src : null
                };

                // Compare with the current metadata
                if (!currentMetadata ||
                    currentMetadata.title !== data.title ||
                    currentMetadata.artist !== data.artist ||
                    currentMetadata.album !== data.album ||
                    currentMetadata.url !== data.url) {

                    console.log("Metadata changed. Preparing to send:", JSON.stringify(data));

                    // Send metadata if socket is open, otherwise buffer it
                    if (socket.readyState === WebSocket.OPEN) {
                        socket.send(JSON.stringify(data));
                    } else {
                        console.log("WebSocket not open. Buffering metadata:", JSON.stringify(data));
                        bufferedMetadata = data; // Store metadata in buffer
                    }

                    // Update current metadata to the new one
                    currentMetadata = data;
                }
            }
        }, 1000); // Check for metadata every second
    }

    // Try connecting when the script is loaded
    connectWebSocket();
})();
