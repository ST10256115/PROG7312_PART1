# Municipal Services App — Windows Forms (.NET Framework)

> A desktop application that helps residents report municipal issues, browse local events & announcements, and track service request status.  
> Built with **C# (WinForms)** using **custom data structures** (DynamicArray, HashTable, Queue/Stack, PriorityQueue/Heap, BinaryTree, **AVL Tree**, Graph + **MST**), persisted to JSON.

---

## Demo Video

**YouTube:** <ADD-YOUR-LINK-HERE>

---

## What this project does

- **Report Issues**  
  Log a service problem (location, category, description), attach files, and choose a contact channel (In-App/SMS/WhatsApp). Generates a **unique reference ID** and saves to `issues.json`.

- **Local Events & Announcements**  
  Browse/search for municipal notices and events by text, category, and date range. Shows **suggested categories** based on your recent searches.

- **Service Request Status**  
  Find a request by reference, view the **status timeline**, see the **department route**, show **Top-K oldest open issues** (heap), and compute a **Minimum Spanning Tree (MST)** over open-issue locations.  
  Also includes an **AVL in-order view** to demonstrate balanced-tree indexing by `CreatedAt`.

---

## Architecture (high level)

MunicipalServicesApp/
├── DataStructures/
│ ├── DynamicArray.cs # custom resizable array
│ ├── CustomQueue.cs # FIFO queue
│ ├── CustomStack.cs # LIFO stack
│ ├── HashTable.cs # key → value map
│ ├── PriorityQueue.cs # binary heap
│ ├── MinHeap.cs # heap primitive
│ ├── BinaryTree.cs # ordered tree + traversals
│ ├── AvlTree.cs # self-balancing BST (CreatedAt index)
│ └── Graph.cs # BFS + utilities
├── Data/
│ ├── IssueStore.cs # JSON persistence for issues
│ ├── IssuePriorityQueue.cs # Top-K oldest open issues (heap)
│ ├── IssueAvlIndex.cs # AVL index by CreatedAt
│ ├── EventStore.cs # events, search, recommendations
│ ├── DepartmentNetwork.cs # department route (graph)
│ ├── LocationNetwork.cs # coordinates + MST
│ └── StatusIndex.cs # per-issue status timeline (BinaryTree)
├── Models/
│ ├── Issue.cs, EventItem.cs, IssueStatus.cs, StatusNode.cs
├── Forms/
│ ├── MainForm.cs # menu with “cards”
│ ├── ReportIssueForm.cs
│ ├── LocalEventsForm.cs
│ └── ServiceRequestStatusForm.cs
├── issues.json # created at runtime
├── events.json # seeded / created at runtime
└── App.config, Program.cs, packages.config


---

## Prerequisites

- **Windows + Visual Studio 2019/2022**
- **.NET Framework 4.7.2 or 4.8** (WinForms)
- **NuGet** package: `Newtonsoft.Json`

> Install the package in Visual Studio:  
> `Project` → **Manage NuGet Packages** → **Browse** → `Newtonsoft.Json` → **Install**.

---

## How to clone

```bash
git clone <YOUR-REPO-URL>
cd MunicipalServicesApp
Open the solution in Visual Studio:

MunicipalServicesApp.sln

## Prerequisites

- Windows with **Visual Studio 2019/2022** installed  
- **.NET Framework** targeting pack: **4.7.2** or **4.8**  
- NuGet package: **Newtonsoft.Json** (Json.NET)

> Install via Package Manager Console (if needed):  
> `Install-Package Newtonsoft.Json`

---

## How to Build & Run

1. **Open** the solution in **Visual Studio**.  
2. **Target Framework**: set to **.NET Framework 4.7.2 or 4.8**  
   - `Project → Properties → Application → Target framework`
3. **Dependencies**: confirm **Newtonsoft.Json** is installed (see prerequisites).
4. **Build**: `Build → Build Solution` (or **Ctrl+Shift+B**).  
5. **Run**: `Debug → Start Debugging` (or **F5**).

**On launch**, the Main Menu appears with three cards:

- **Report an Issue** *(Live)*
- **Local Events & Notices** *(Live)*
- **Service Request Status** *(Live)*

---

## Using the App

### Report Issues
- Enter **Location**, pick a **Category**, and add a **Description**.  
- Optional: click **Attach Images/Documents…** to add files.  
- Choose a contact channel (**In-App/SMS/WhatsApp**) and a **phone number** if needed.  
- Click **Submit** → a **progress bar** shows submission; a **message box** confirms your **Reference ID**.  
- Attachments are copied to: `./Attachments/<IssueId>/` *(demo)*.

### Local Events & Announcements
- **Search** by text, **filter** by category, and optionally pick **From/To** dates.  
- **Suggested** shows top categories from your **recent searches**.  
- Data is **seeded** into `events.json` on first run.

### Service Request Status
- Enter a **Reference ID (GUID)** to locate a request.  
- View **Status Timeline**, **Department Route**, and **Advance Status** suggestions.  
- **Top-K** oldest open issues (choose **K** and click **Top-K**).  
- **Compute MST** to see total connection length and edges.  
- **AVL (in-order)** shows the first **N** issues in ascending `CreatedAt` using the AVL index (**N = Top-K value**).

---

## Data Structures & Algorithms (for examiners)

- `DynamicArray` – core resizable storage for issues/attachments.  
- `HashTable` – **category → event list** mapping for fast lookup.  
- `CustomQueue` / `CustomStack` – capture search history & recency.  
- `PriorityQueue` / `MinHeap` – efficient **Top-K** oldest open issues.  
- `BinaryTree` – timeline storage + **in-order traversal** for per-issue history.  
- `AVLTree` – global index of issues by `CreatedAt` (balanced **O(log n)** ops), surfaced via **AVL (in-order)**.  
- **Graph + BFS** – department routing; **MST** over open-issue location graph.

> All structures are **custom-built** (no `List<T>`, `Dictionary<TKey,TValue>`, etc. in rubric-critical areas).

---

## Data Files

- `issues.json` – created/updated at runtime.  
- `events.json` – seeded on first run.  
- `Attachments/<IssueId>/` – demo copy of user attachments.

**Resetting data:** close the app and delete `issues.json` (and the `Attachments/` folder) to start fresh.

---

## Keyboard & UX Tips

- **Esc** closes child forms.  
- Tooltips describe disabled features (where applicable).  
- Buttons use hover states; UI palette is consistent (municipal-branded).

---

## Troubleshooting

- **Designer not used?** Some forms are **code-first** by design to keep custom layouts clean.  
- **`Newtonsoft.Json` not found:** Ensure the package is installed **for this project** (not just the solution).  
- **Old JSON incompatible:** Delete `issues.json` if you change model shapes during development.  
- **Reference not found:** Ensure you pasted the exact **GUID** from the submission confirmation.

---

## Academic Notes

- Project implements and demonstrates: **Trees (Binary/AVL), Heaps, Graphs, BFS, MST, Stacks/Queues, HashTable, Set**.  
- The **Status** module explicitly surfaces **Top-K (heap)** and **AVL in-order** for verifiable algorithmic behaviour.  
- Code is **C# 7.3** compatible (WinForms, .NET Framework 4.7.2/4.8) with **separation of concerns**.

---

## License / AI Usage

- Add a license file if required (e.g., **MIT**).  
