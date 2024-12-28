# YT Music Discord RPC

A desktop application that integrates **Discord Rich Presence** with **YouTube Music**. This app listens to metadata from YouTube Music, including the track title, artist, album, artwork, and track URL, and updates the user's Discord status accordingly.

## Features

- Displays **Discord Rich Presence** with track information such as:
  - Track title, artist, album, and artwork.
  - A "Listen on YouTube Music" button that links directly to the current track on YouTube Music (Only visible to other users).
- Uses **WebSocket** for communication between the **browser script** (running in YouTube Music) and the **.NET desktop application**.

## Requirements

### For the Desktop Application

- **.NET Framework 4.7.2** or higher
- **WebSocketSharp** - WebSocket server to communicate with the browser
- **Newtonsoft.Json** - For JSON parsing

### For the Browser Script

- **Tampermonkey** or **Greasemonkey** installed in your browser.
  
### Libraries Used

- **[DiscordRPC](https://github.com/Lachee/discord-rpc-csharp)** - For interacting with Discord's Rich Presence API.
- **[WebSocketSharp](https://github.com/sta/websocket-sharp)** - For setting up the WebSocket server.
- **[Newtonsoft.Json](https://www.newtonsoft.com/json)** - For JSON serialization/deserialization.

## Installation

### Desktop Application

1. Clone this repository or download the project.
2. Open the solution in **Visual Studio** (or any compatible .NET IDE).
3. Build the project using **Release** mode.
4. Grab the latest **DiscordRPC.dll**, and add it as a reference to the project. See [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp/releases) for more details.

### Browser Script

1. Install **Tampermonkey** (or **Greasemonkey**) in your browser.
2. Create a new user script and copy the contents from the `YT Discord RPC` script located in the `/browser-script` directory or provided in the documentation.
3. Ensure the script is enabled for **YouTube Music**.
4. Make sure that the WebSocket server is running before using the script.

## Usage

1. **Run the Desktop Application**: Open the compiled `YTRPC.exe` application. It will start a WebSocket server on `ws://localhost:5000/ws` and wait for connections.
   
2. **Connect the Browser Script**:
   - The browser script automatically connects to the WebSocket server when you visit YouTube Music.
   - It will continuously monitor the song metadata and send it to the desktop app for updating the Discord Rich Presence.

3. **Interact with the Discord Presence**:
   - You will see your **track title, artist, and album** displayed on your Discord profile.
   - A **"Listen On YouTube Music"** button will appear, which allows others to listen to the same track on YouTube Music.

4. **Exit the Application**:
   - Right-click the tray icon and click **Exit** to close the application.

## Troubleshooting

- **No Rich Presence shown**: Ensure the desktop application is running, and the browser script is enabled and connected to the WebSocket server along with the Discord app being open.
- **WebSocket errors**: If the WebSocket connection fails or disconnects, the script will attempt to reconnect automatically. Ensure that the WebSocket server is running on the correct port (`5000`).
- You can see some basic logging in the browser console under developer tools
  
## Contributing

Contributions are welcome! Feel free to fork the repository, open issues, or submit pull requests. This project was made for fun, so excuse any code slop 😅

### How to contribute:

1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Commit your changes.
4. Push your changes and open a pull request.

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## Acknowledgements

- [DiscordRPC-CSharp](https://github.com/Lachee/discord-rpc-csharp/) - For the Discord RPC library.
- [WebSocketSharp](https://github.com/sta/websocket-sharp) - For WebSocket support.
- [Tampermonkey](https://www.tampermonkey.net/) - For running the browser script.
