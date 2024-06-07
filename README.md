# OrderManager

## Note: This project is not finished and is still in progress

I started creating a wpf program, which can communicate with a sql database to create, delete and change orders. In the database I implemented orders and processes, where any order can have several processes. Until now this program only interacts with orders.

The main purpose of this program is for myself to learn more about mvvm patterns and object orientation. As I started developing, I got more and more feature ideas, which I just implemented for fun. The more complex the project becomes, the more I realise that I have to adhere to the solid principles.

## Features
- Listview with the order entries.
- Create, delete and update orders.
- Editable order number and order id.
- Self created pagination with X items shown in list.
- Live search for orders.

## Tbd
- Fix some pagination bugs -> If an order is created or deleted while searching, the pagination is gets into not comprehensible behavior.
- Implement processes, where each order contains several processes
- Switching users with different rights of delete/ change/ create orders/ processes

Here I provided some screenshots with the features.

![main](https://github.com/oadriano/OrderManager/assets/39732702/bc2f881c-72aa-40ae-986f-df6a9ad8fe29)

![selected](https://github.com/oadriano/OrderManager/assets/39732702/69f234dd-3fbe-46d5-98b6-60afbd2dbe99)

![search](https://github.com/oadriano/OrderManager/assets/39732702/41256ba8-487b-4125-99f0-fa92bc7be04e)


# Next project: Using this sql script to manage it with a blazor web app
