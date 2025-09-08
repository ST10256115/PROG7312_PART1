# Municipal Services App – Windows Forms (.NET Framework)

##  Overview
This application is part of the PoE project to improve municipal service delivery in South Africa.  
It demonstrates core functionality for **Task 2: Application Development**.  

The app provides a **Main Menu** with three options:  
- **Report Issues** (Implemented)  
- **Local Events & Announcements** (Coming Soon – disabled)  
- **Service Request Status** (Coming Soon – disabled)  

---

##  Video Demonstration
--Watch the demo of the application here:  
--[YouTube Demo](https://youtu.be/LN55laE0w9k)  

##  How to Clone the Repository

git clone <your-repo-url>
cd MunicipalServicesApp
Replace <your-repo-url> with your GitHub repository link.

--Open the cloned folder in Visual Studio 2022.

--Open the solution file: MunicipalServicesApp.sln.

## How to Compile
--Ensure the project type is Windows Forms App (.NET Framework).
--Install NuGet package: Newtonsoft.Json
--Right-click project → Manage NuGet Packages → Browse → search Newtonsoft.Json → Install.
--Build → Build Solution (Ctrl+Shift+B).

## How to Run
--Press F5 or click Start Debugging.
--The Main Menu will load.
--Click Report Issues to open the reporting form.

## How to Use “Report Issues”
--Enter a Location.
--Select an Issue Category from the dropdown (Sanitation, Roads, Electricity, etc.).
--Provide a Description of the problem.
--(Optional) Attach images/documents using the Attach button.
--Choose a Preferred Contact Channel (In-App, SMS, or WhatsApp).
--Enter a phone number if SMS/WhatsApp is selected.
--Click Submit.
--A progress bar will show submission progress.
--A confirmation message will display with a unique reference ID.

## Data Handling
--Reports are saved into a JSON file:
--issues.json in the project’s root folder.
--Attachments are copied to:
--./Attachments/<IssueId>/ for demonstration.
--Data structure used: custom DynamicArray<Issue>, with persistence via JSON DTO mapping.

## Features Implemented (Task 2 Requirements)
--Main Menu with 3 tasks (only “Report Issues” enabled).
--Report Issue Form with all required inputs (Location, Category, Description, File Attachment).
--User Feedback via progress bar, tips label, and confirmation messages.
--Engagement strategy from Task 1 integrated:
--Contact channel options (In-App, SMS, WhatsApp).
--Tips and progress updates to keep users informed.
--Navigation back to main menu.
