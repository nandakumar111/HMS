# Hotel Booking System

Implement a very simple hotel booking system for the following use cases.

### [Application Reference Images](https://github.com/nandakumar111/HMS/tree/main/HotelManagementSystem/Images/AppReference)

## Hotel Owner Use Cases

* As a hotel owner, I should be able to add new rooms so that customers can book it.
* As a hotel owner, I should be able to see the status of all rooms (Booked/Not Booked), for a selected date so that I can make necessary preparations to accommodate the guests.

## Customer Use Cases

* As a customer, by entering my email ID, I should be able to book a room, by selecting a time period and the room type (Single/Double). If the room is available for the selected period, then the system should book the room and display a success message with the booked room no, otherwise, it should display an appropriate message indicating that rooms are not available.

# Required pages

* ### Rooms List Page

    * Contains a date control and a submit button.

    * When user selects a date and clicks on the submit button, the system will fetch all the rooms and display the status of the rooms (Booked / Not Booked) for the selected date in a grid view. The necessary columns are Room No, Room Type and Status

    * The rooms list page should also have a Add Room button. Clicking on it should navigate to the Add Room Page

* ### Add Room Page

    * Add Room page is a simple forms page where the hotel owner can add rooms. It should have Room No (Text), Room Type (Dropdown – Single / Double) and a submit button.

    * On clicking on submit, the room should be added to the database.

    * Note that Room No must be a unique string.

* ### Book Room Page

    Should have the following controls

    * Email Id (Text Box)

    * From date (Date Control)

    * To date (Date Control)

    * Room Type (Dropdown – Single / Double)

    * Button (submit)

    Once the user enters all the details and clicks on submit button, the system should check if there are any rooms in Not Booked status for the given conditions (date and room type). If the room is available, then the system will book the room and return the room no to the user with a success message. Otherwise, the system should show an appropriate error message.
* ### Development instructions

  Use either SQL Server or Mongo DB. The application must be developed using Angular (latest version) and Web API (.NET Core)

  Important Instructions:

  - [ ] Add validations wherever necessary (EST : 15 Oct 2022)

  - [x] Code must be compilable and executable

  - [x] Should provide db scripts for the database items or a database backup file (.bak)

  - [x] Any other special instructions should be clearly specified.

  - [x] Should not zip with library files (.dll or node modules) they will be built during compilation.