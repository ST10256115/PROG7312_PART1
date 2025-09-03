# Municipal Services App – Windows Forms (.NET Framework)

##  Overview
This application is part of the PoE project to improve municipal service delivery in South Africa.  
It demonstrates core functionality for **Task 2: Application Development**.  

The app provides a **Main Menu** with three options:  
- **Report Issues** (Implemented)  
- **Local Events & Announcements** (Coming Soon – disabled)  
- **Service Request Status** (Coming Soon – disabled)  

---

##  How to Clone the Repository

git clone <your-repo-url>
cd MunicipalServicesApp


* Replace `<your-repo-url>` with your GitHub repository link.
* Open the cloned folder in **Visual Studio 2022**.
* Open the solution file: `MunicipalServicesApp.sln`.

---

##  How to Compile

1. Ensure the project type is **Windows Forms App (.NET Framework)**.
2. Install NuGet package: **Newtonsoft.Json**

   * Right-click project → *Manage NuGet Packages* → Browse → search `Newtonsoft.Json` → Install.
3. Build → Build Solution (**Ctrl+Shift+B**).

---

## ▶ How to Run

* Press **F5** or click **Start Debugging**.
* The **Main Menu** will load.
* Click **Report Issues** to open the reporting form.

---

##  How to Use “Report Issues”

1. Enter a **Location**.
2. Select an **Issue Category** from the dropdown (Sanitation, Roads, Electricity, etc.).
3. Provide a **Description** of the problem.
4. (Optional) **Attach images/documents** using the Attach button.
5. Choose a **Preferred Contact Channel** (In-App, SMS, or WhatsApp).

   * Enter a phone number if SMS/WhatsApp is selected.
6. Click **Submit**.

   * A **progress bar** will show submission progress.
   * A confirmation message will display with a **unique reference ID**.

---

##  Data Handling

* Reports are saved into a JSON file:
* `issues.json` in the project’s root folder.
* Attachments are copied to:
* `./Attachments/<IssueId>/` for demonstration.
* Data structure used: **List<Issue>**, with persistence via JSON.

---

##  Features Implemented (Task 2 Requirements)

* **Main Menu** with 3 tasks (only “Report Issues” enabled).
* **Report Issue Form** with all required inputs (Location, Category, Description, File Attachment).
* **User Feedback** via progress bar, tips label, and confirmation messages.
* **Engagement strategy** from Task 1 integrated
* Contact channel options (In-App, SMS, WhatsApp).
* Tips and progress updates to keep users informed.
* **Navigation** back to main menu.

---

## Known Limitations

* SMS/WhatsApp sending is **not implemented** (recorded only).
* Local Events and Service Request Status are **disabled** placeholders.
* No authentication or real backend integration (demo only).

