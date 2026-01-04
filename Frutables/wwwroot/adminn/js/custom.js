let loadMore = document.querySelector(".load-more");

loadMore.addEventListener("click", function () {
    let htmlProductCount = document.querySelectorAll(".load-products .product").length;
    let dbProductCount = document.querySelector(".load-products .product-count").value;

    fetch(`Home/LoadMore?skip=${htmlProductCount}`)
        .then(response => response.text())
        .then(response => {
            let parent = document.querySelector(".load-products");
            parent.innerHTML += response;

            htmlProductCount = document.querySelectorAll(".load-products .product").length;

            if (htmlProductCount == dbProductCount) {
                this.style.display = "none";
            }
        });
});

document.addEventListener("DOMContentLoaded", function () {

    const buttons = document.querySelectorAll(".btn.btn-primary[data-id]")

    buttons.forEach(button => {
        button.addEventListener("click", function () {

            const id = this.dataset.id;

            fetch("/basket/add", {
                method: "POST",
                headers: {
                    "Content-Type": "application/x-www-form-urlencoded"
                },
                body: "id=" + id
            })
                .then(res => {
                    if (!res.ok) throw new Error("Request failed");
                    return res.json();
                })
                .then(data => {
                    console.log("Total count:", data.count);
                    alert("Product added to basket");
                })
                .catch(err => console.error(err));
        });
    });

});