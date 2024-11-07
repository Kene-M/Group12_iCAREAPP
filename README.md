# Group12_iCAREAPP
iCARE is a web application built using the ASP.NET MVC Framework, designed for managing healthcare tasks, documents, patient and treatment records efficiently.

## Project Details
**Created by:** Kene, Isaiah, Sydney, Cooper.

**Name of Database:** Group12_iCAREDB

### Description
The iCARE application provides a user-friendly interface for managing patients, assigning healthcare workers, and handling document storage. This tool is intended to streamline administrative tasks and improve the efficiency of healthcare workflows.

* **Patient Record Management**: Create, view, edit, and delete patient records, with fields for essential information such as height, weight, date of birth, and assigned bed ID.
* **Worker Assignments**: Assign doctors and nurses to patients, with limits enforced (e.g., only one doctor and up to three nurses per patient).
* **Document Handling**: Store and view patient-related documents, with support for inline display of images and PDFs.
* **Minimal Role-Based Access Control**: Different levels of access and views have been assigned to doctors, nurses, and administrators to maintain data security and integrity.

## How to Set Up and Run the Application
### Database Setup
* Download Group12_iCAREDB.bak, the database backup file for the project.
* Right-click on the file, go to Properties, and unblock it if necessary.
* In Microsoft SQL Server Management Studio (SSMS), restore the database by navigating to Databases > Restore Database..., and select Group12_iCAREDB.bak.
* Ensure that the database is connected in SSMS after restoration.

### Application Setup in Visual Studio
* Open the solution file for the Group12_iCAREAPP in Visual Studio.
* Update the database connection string in the configuration file (web.config) to reflect your SQL Server instance name.
  * **Example**: Change {*connectionStrings*} values to your server’s name in Server=YourServerName;Database=Group12_iCAREDB;...
* Rebuild the solution to ensure there are no configuration errors.

### Running the Application
* Once configured, run the application directly from Visual Studio to launch the iCARE web interface.
* Log in and navigate through the various sections for managing patients, assigning workers, and handling document storage.
