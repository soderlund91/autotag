<img width="512" height="130" alt="autotag" src="https://github.com/user-attachments/assets/3ae7b44c-12bd-4b61-ad1a-56987e57c110" />



# Emby AutoTag Plugin 🏷️

**AutoTag** is a powerful plugin for Emby Server that automatically manages tags for your Movies and TV Series based on dynamic lists from **Trakt** and **MDBList**.

Stop manually tagging "Trending" or "Recommended" content. Let AutoTag keep your library in sync with the world.

Perfect for Emby 4.10+, this plugin pairs perfectly with the new Home Screen management in Emby Server v4.10.0.1+. It allows you to populate your home screen based on Tags instead of cluttering your library with temporary Collections.
Using tags is a perfect way to manage and sort your media without cluttering your library.

## 🚀 Key Features

### 🔄 True Synchronization
AutoTag doesn't just add tags; it **enforces** them.
* If a movie enters your Trakt list -> **Tag Added**.
* If a movie *leaves* your Trakt list -> **Tag Removed**.
* Your library always reflects the current state of the list.

### 🧠 Self-Cleaning Memory
Changed your mind? If you delete a tag configuration from the settings, AutoTag remembers and **automatically wipes** that tag from your entire library on the next run. No manual cleanup required.

### 🔗 Robust Source Support
* **Trakt:** Supports User Lists, Watchlists, and Official Lists (Trending, Popular, Watched).
* **MDBList:** Full support for dynamic JSON lists.
* **Smart Parsing:** Handles complex URLs automatically.


### Screenshot
<img width="783" height="963" alt="autotag" src="https://github.com/user-attachments/assets/c9d8f702-5647-41ea-9dff-d57b20b1f208" />




---

## 📦 Installation

1.  Download the latest `.dll` from the [Releases](../../releases) page.
2.  Shut down your Emby Server.
3.  Place the `.dll` file in your Emby plugins folder.
4.  Start Emby Server.

---

## ⚙️ Configuration

Go to your Emby Dashboard. You will see **Auto Tag** in the sidebar menu (usually under the *Server* or *Settings* section).

### 1. API Keys
* **Trakt Client ID:** Required if you use Trakt lists. You can get one for free at [Trakt API](https://trakt.tv/oauth/applications).
* **MDBList API Key:** Required if you use MDBList. Get one for free at  [MDBList.com](https://mdblist.com/developer/)

### 2. Adding Sources
Click **+ Add Source** to create a new sync task.

| Setting | Description |
| :--- | :--- |
| **Active** | Toggle to enable/disable this source without deleting it. |
| **Tag Name** | The actual tag applied to your media (e.g., `weekly_trending`). |
| **Source URL** | The URL to the list (see examples below). |
| **Limit** | Max number of items to tag from this list (e.g., Top 50). |

### 🔗 Supported URL Examples

**Trakt:**
* **Trending Movies:** `https://trakt.tv/movies/trending`
* **User List:** `https://trakt.tv/users/username/lists/my-awesome-list`
* **Watched (Weekly):** `https://trakt.tv/movies/watched/weekly`

**MDBList:**
* **Dynamic List:** `https://mdblist.com/lists/user/listname/`

---

## 🕒 How it works

The plugin creates a Scheduled Task in Emby called **"AutoTag: Sync Tags"**.

* **Default Schedule:** Runs daily at 04:00 AM.
* **Manual Run:** You can run it anytime via *Settings -> Scheduled Tasks -> Library*.

### The Logic Flow
1.  **Memory Check:** Detects if you deleted any tags from the config and cleans them up.
2.  **Global Wipe:** Temporarily removes the target tag from *all* items (ensuring fresh sync).
3.  **Fetch & Match:** Downloads the list, matches items via IMDB/TMDB IDs.
4.  **Apply:** Tags the matched items.

---

## ❓ Troubleshooting

**Q: I removed a tag from the settings, but it's still on my movies?**
A: Run the "AutoTag: Sync Tags" scheduled task one more time. The plugin needs one run to detect the deletion and perform the cleanup.

**Q: My Trakt list isn't syncing.**
A: Ensure your **Trakt Client ID** is correct and that the list is public. Check the Emby logs for "AutoTag" to see detailed error messages.

**Q: Does this work with TV Shows?**
A: Yes! It supports both Movies and Series.

---

**Disclaimer:** This plugin is not affiliated with Emby, Trakt, or MDBList. Use at your own risk.
