

<img width="481" height="142" alt="HSC" src="https://github.com/user-attachments/assets/2887420a-7198-472e-8afb-a49eda3231d9" />


# HomeScreenCompanion — (Former AutoTag)

**HomeScreenCompanion** (formerly AutoTag) is a powerful plugin for Emby Server that automatically manages tags and collections for your Movies and TV Shows—and keeps your home screen sections in sync across multiple users.

> **v3.0.0.0** — Plugin renamed from AutoTag to Home Screen Companion.

> [!IMPORTANT]
> Since version 3.0.0.0 this plugin no longer support Emby server version 4.9.x.x. Requires Emby 4.10+

---

## 🚀 Key Features

### 🌟 Dynamic Lists from Trakt & MDBList
Make your library feel alive with tags that update automatically every day.
* **Trending Now** — always shows what's currently hot worldwide.
* **Popular & Top Rated** — self-managing "Best of" lists.
* **Personal Curations** — sync your own Trakt/MDBList lists (e.g., "Best of 80s", "Dad's Favorites").
* **Automagical Sync** — link a URL, and the plugin handles the rest. Items are added and removed automatically.

### 📚 Automated Collection Management
Beyond tagging, the plugin can manage your Emby Collections.
* **Auto-Create** — create a corresponding Emby Collection while a list is active.
* **Dynamic Membership** — items join/leave the collection as the list changes.
* **Only Collection Mode** — enable "Disable Item Tagging" to keep item metadata clean while still enjoying organized collections.
* **Smart Removal** — when a collection is disabled or its schedule ends, the plugin automatically deletes it from Emby.

### 🧩 Local Sources: Collections & Playlists
No external API needed. Use what's already in your library.
* Tag items based on existing **Emby Collections** or **Playlists**.
* Combine with schedules to create time-limited promotions of curated content.

### 📅 Advanced Scheduling
Set tags and collections to be active only during specific time windows. AutoTag handles cleanup when the window closes.
* **Annual** — e.g., "Christmas Movies" active Dec 1–31 every year.
* **Weekly** — e.g., "Friday Movie Night" active only on Fridays.
* **Specific Dates** — for one-time events or marathons.

### 🔬 MediaInfo Filtering
Tag items based on their actual technical properties—no manual curation required.
* **Rule-based UI** — build filters from a property dropdown with context-sensitive value inputs.
* **Property categories** — Video (4K, HDR, HEVC, H.264, SD), Audio (Atmos, DTS-HD MA, AC3, AAC, Stereo, Mono, channels), Content (Genre, Studio, Actor/Director, Rating, Language), Metrics (Year, Runtime, Community Rating, Critic Rating).
* **Numeric operators** — `>`, `>=`, `<`, `<=`, `=` for ratings, year, runtime, and channel count.
* **Text matching** — case-insensitive contains for Genre, Studio, Actor, Director, and Language.
* **AND/OR within groups** — combine multiple criteria per group with AND or OR logic.
* **AND/OR between groups** — chain multiple criterion groups together.
* Backward compatible with previously saved configs.

**Example use cases:**
* Tag all 4K Atmos movies → `4K AND Atmos`
* Tag action movies from 2020+ with a rating above 7 → `Genre: Action AND Year >= 2020 AND CommunityRating >= 7`
* Tag content suitable for a specific setup → `DTS-HD MA OR Atmos`

### 🏠 Home Screen
Two complementary features let you fully control home screen sections for all your users.

#### Per-entry Home Screen Sections
Add a dedicated home screen section directly from any tag/collection entry — without touching Emby's own home screen settings.

* **Single Collection** — creates a `boxset` section that shows the items in the entry's managed Emby Collection. Requires *Create Collection* to be enabled on the entry.
* **Dynamic Media (tag)** — creates an `items` section filtered by the entry's tag, so only tagged items appear. Requires *Enable Tag* to be enabled.
* **Item Types** — choose which item types to include (Movie, Series, Episode, MusicVideo).
* **Custom Title, Image Type, Sort By, Sort Order, Scroll Direction** — all configurable per section.
* **Target users** — select which users get the section.
* The section is automatically **created, updated, and removed** each time the sync task runs — no manual work needed.

#### Home Screen Copy (Manage tab)
Keep home screen sections in sync across all your Emby users—automatically.
* **Source user** — pick one user whose home screen layout is the "master" configuration.
* **Target users** — select one or more users whose home screens will be updated to mirror the source.
* **Scheduled task** — runs every 30 minutes by default, or manually trigger a sync from the plugin page.
* **Live status** — the plugin page shows last sync time, result, and how many sections were copied.

This is ideal for multi-user households or Emby servers with guests, where you want everyone to see the same curated home screen without manually configuring each account.

### 🔄 True Synchronization
AutoTag doesn't just add tags; it **enforces** them.
* Item enters your list → **Tag/Collection Added**.
* Item leaves your list → **Tag/Collection Removed**.
* Your library always reflects the current state of the list.

### 🛡️ Safety & Stability
* **Fail-Safe Cleanup** — if a remote list fails to download, the plugin skips cleanup for that tag/collection to prevent accidental data loss.
* **Dry Run Mode** — test your configuration in the logs without modifying anything in your library.
* **Live Logging** — view the execution log directly inside the plugin settings with real-time status updates.

---

## 📦 Installation

1. Download the latest `.dll` from the [Releases](../../releases) page.
2. Shut down your Emby Server.
3. Place the `.dll` file in your Emby plugins folder.
4. Start Emby Server.

---

## ⚙️ Configuration

Go to your Emby Dashboard. You will see **Home Screen Companion** in the sidebar menu.

The plugin page has three tabs: **Tag and Collection**, **Home Screen** and **Settings**

### API Keys (Tag and Collection tab)
* **Trakt Client ID** — required for Trakt lists. Get one free at [trakt.tv/oauth/applications](https://trakt.tv/oauth/applications).
* **MDBList API Key** — required for MDBList. Get one free at [mdblist.com/developer](https://mdblist.com/developer/).

### Managing Sources

Each source is configured across four tabs:

#### 🏠 Source Tab
| Setting | Description |
| :--- | :--- |
| **Active** | Toggle to enable/disable this source. |
| **Tag Name** | The tag applied to matching media (e.g., `weekly_trending`). |
| **Limit** | Max number of items to tag/collect from this list. |
| **Source URL / Selection** | URL (Trakt/MDBList), or local Collection/Playlist name. |

#### 📅 Schedule Tab
| Setting | Description |
| :--- | :--- |
| **Schedule Rule** | Define when this source should be active. |
| *Specific Date* | Active only between a Start and End date. |
| *Recurring* | Repeats every year (e.g., Dec 1–31). |
| *Week Days* | Active only on selected days of the week. |

#### 📚 Collection Tab
| Setting | Description |
| :--- | :--- |
| **Create Collection** | Automatically maintain an Emby Collection for this list. |
| **Collection Name** | Optional custom name (defaults to Tag Name). |
| **Disable Tagging** | Add items to the collection without tagging them individually. |

#### 🔧 Advanced Tab
| Setting | Description |
| :--- | :--- |
| **Blacklist** | Comma-separated IMDB IDs to ignore (e.g., `tt1234567`). |
| **MediaInfo Filter** | Rule-based filter to restrict which items in the list get tagged. |

#### 🏠 Home Screen Tab
| Setting | Description |
| :--- | :--- |
| **Add as home screen section** | Enable to let the plugin manage a home screen section for this entry. |
| **Section Type** | *Single Collection* — shows the managed collection as a boxset row. *Dynamic Media (tag)* — shows items filtered by this entry's tag. |
| **Item Types** | Which item types to include in the section (Movie, Series, Episode, MusicVideo). Applies to Dynamic Media only. |
| **Custom Title** | Override the section's display name on the home screen. |
| **Image Type** | Card image style: Default, Primary, Backdrop, or Thumb. |
| **Sort By** | How items are ordered: Default, Rating, Date Added, Name, Runtime, Release Date, Year, or Random. |
| **Sort Order** | Ascending or Descending (Default lets Emby decide). |
| **Scroll Direction** | Horizontal or Vertical scroll (Default lets Emby decide). |
| **Target Users** | Which users get this home screen section. |

> **Note:** For *Single Collection*, the entry must have *Create Collection* enabled. For *Dynamic Media (tag)*, the entry must have *Enable Tag* enabled.

---

### 🏠 Home Screen tab (Home Screen Companion)

The **Home Screen** tab at the top of the plugin page has two sub-tabs:

#### Manage
Copy the entire home screen layout from a source user to one or more target users.

| Setting | Description |
| :--- | :--- |
| **Enable sync** | Turn the copy feature on or off. |
| **Source user** | The user whose home screen layout is used as the master. |
| **Target users** | Users whose home screens will be overwritten to match the source. |

#### Copy
Manually trigger or view the status of the home screen copy sync. Shows last run time, result, and number of sections copied.

---

### 🔗 Supported URL Examples

**Trakt:**
* Trending Movies: `https://trakt.tv/movies/trending`
* Popular Movies: `https://trakt.tv/movies/popular`
* Watched (Weekly): `https://trakt.tv/movies/watched/weekly`
* User List: `https://trakt.tv/users/username/lists/my-awesome-list`

**MDBList:**
* Dynamic List: `https://mdblist.com/lists/user/listname/`

---



### Screenshots

<img width="775" height="828" alt="image" src="https://github.com/user-attachments/assets/bfe32910-ad40-418b-8c2f-a4b38fe7b69a" />

<img width="721" height="1200" alt="image" src="https://github.com/user-attachments/assets/b96714b8-d9bf-4aea-9a66-bc08e7060d48" />

<img width="778" height="905" alt="image" src="https://github.com/user-attachments/assets/04b29706-1e23-4505-b2e0-155089bdb433" />

<img width="916" height="699" alt="image" src="https://github.com/user-attachments/assets/b78d6f9a-6ce5-4af7-b4d5-f05c11abcbef" />


---
**Disclaimer:** This plugin is not affiliated with Emby, Trakt, or MDBList.
This plugin is heavily vibe-coded, tested on my own server — use at your own risk.
