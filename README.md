#  Municipal Services App — Windows Forms (.NET Framework)

## Overview
The **Municipal Services App** is developed as part of the **INSY7314 PoE Project** to enhance municipal service delivery and citizen engagement through technology.  
It allows users to **report local issues**, **view municipal events and notices**, and eventually **track service requests**.  
The system demonstrates practical use of **custom-built data structures** within a professional, interactive Windows Forms environment.

This project fulfills **Part 1** (application development and engagement strategies) and **Part 2** (enhanced data structures and event management).

---

## Features Overview

### Part 1 – Issue Reporting Module
- **Report Issues**: Submit problems such as water leaks, road damage, power outages, etc.  
- **Attachments**: Upload images or documents to support reports.  
- **Preferred Contact Channels**: In-App, SMS, or WhatsApp.  
- **Confirmation Feedback**: Progress bar, success messages, and reference ID display.  
- **Data Persistence**: All submissions saved in `issues.json` for record-keeping.  

### Part 2 – Local Events & Announcements
- **Event Listing & Search**: Displays upcoming community meetings, maintenance notices, and outages.  
- **Category Filtering**: Filter events by type (Utilities, Safety, Roads, etc.).  
- **Date Range Search**: Find events within custom date ranges.  
- **Engagement Tracker**: Uses custom stacks and queues to record user searches.  
- **Recommendations**: Suggests top-searched event categories using hash-based frequency counts.  
- **Data Persistence**: `events.json` file stores pre-seeded and dynamically loaded event data.  

---

## Technical Implementation

### Custom Data Structures Used
All data structures were **custom-built** (no generics or built-in collections):

| Data Structure | Class | Usage |
|----------------|--------|--------|
| **DynamicArray\<T\>** | `DynamicArray.cs` | Stores lists of issues or events dynamically. |
| **HashTable\<K,V\>** | `HashTable.cs` | Indexes events by category and tracks search frequencies. |
| **CustomStack\<T\>** | `CustomStack.cs` | Keeps a recent search history for recommendations. |
| **CustomQueue\<T\>** | `CustomQueue.cs` | Maintains queued search terms for background processing. |
| **PriorityQueue\<T\>** | `PriorityQueue.cs` | Orders events chronologically (soonest first). |
| **CustomSet\<T\>** | `CustomSet.cs` | Stores unique event categories without duplicates. |

All structures were implemented manually to demonstrate algorithmic understanding (resizing arrays, handling collisions, and ensuring O(1)–O(log n) average operations).

---

## Application Structure

| Folder | Contents |
|---------|-----------|
| **/DataStructures** | Custom-built data structures for arrays, stacks, queues, etc. |
| **/Data** | `IssueStore.cs` and `EventStore.cs` manage JSON data handling and indexing. |
| **/Models** | Domain models: `Issue.cs` and `EventItem.cs`. |
| **/Forms** | `MainForm.cs`, `ReportIssueForm.cs`, and `LocalEventsForm.cs`. |
| **issues.json** | User-submitted issue records. |
| **events.json** | Municipal events and notices (seeded data). |
| **Attachments/** | Uploaded files organized by issue ID. |

---

git clone <your-repo-url>
cd MunicipalServicesApp
