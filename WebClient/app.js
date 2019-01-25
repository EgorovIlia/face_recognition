(function () {
    var app = new Vue({
        el: '#app',
        data: {
            categories: [],
            exist: false,
            errored: false,
            show: false
            // [{
            //     faces: [{ Age: 3500, Gender: 'Male', Cat: 10, Bitmap: '' },
            //     { Age: 3300, Gender: 'Male', Cat: 10, Bitmap: '' }],
            //     Cat: 10
            // }],
        },
        methods: {
            outputInfo: function (indexCat, indexFace) {
                return this.categories[indexCat].faces[indexFace].Gender +
                    "  " + this.categories[indexCat].faces[indexFace].Age;
            },
            outputCat: function (indexCat) {
                switch (this.categories[indexCat].Cat) {
                    case 0: str = 'Mx00_19'
                        break
                    case 1: str = 'Mx20_39'
                        break
                    case 2: str = 'Mx40_59'
                        break
                    case 3: str = 'Mx60_79'
                        break
                    case 4: str = 'Mx80_'
                        break
                    case 5: str = 'Fx00_19'
                        break
                    case 6: str = 'Fx20_39'
                        break
                    case 7: str = 'Fx40_59'
                        break
                    case 8: str = 'Fx60_79'
                        break
                    case 9: str = 'Fx80_'
                        break
                    case 10: str = 'not_determined'
                        break
                    default:
                        alert('Я таких значений не знаю')
                }
                if (str[0] == 'M')
                    Gender = "Male";
                else if (str[0] == 'F')
                    Gender = "Female";
                startIndex = str.indexOf("x") + 1;
                endIndex = str.indexOf("_");
                AgeFrom = str.substr(startIndex, endIndex - startIndex)
                startIndex = str.indexOf("_") + 1;
                AgeTo = str.substr(startIndex, str.length - startIndex)
                return Gender + " aged from " + AgeFrom + " to " + AgeTo + " years";
            },
            getFaces: function () {
                this.errored = false;
                fetch("http://localhost:60632/api/faces")
                    .then((response) => {
                        return response.json();
                    })
                    .then((d) => {
                        if (this.exist) {
                            this.categories = []
                            this.exist = false
                        }
                        let category_exist = false
                        let currCat = -1;
                        for (let i = 0; i < 10; i++) {
                            category_exist = false
                            for (let j = 0; j < d.length; j++) {
                                if (d[j].Cat == i) {
                                    if (!category_exist) {
                                        this.categories.push({
                                            faces: [],
                                            Cat: i,
                                        });
                                        currCat += 1
                                    }
                                    var face = {
                                        Age: d[j].Age,
                                        Gender: d[j].Gender,
                                        Cat: i,
                                        Bitmap: d[j].Bitmap,
                                    }
                                    this.categories[currCat].faces.push(face)
                                    category_exist = true
                                }

                            }
                        }
                        if (this.categories.length > 0){
                            this.exist = true
                            this.show = true
                        }else{
                            this.show = false
                            this.exist = false
                        }
                    })
                    .catch(err => {
                        console.log("It seems server is unreachable!")
                        this.errored = true;
                        this.show = false
                        this.categories = []
                        this.exist = false
                    })
            }
        }
    });
})();

// (function () {
//     var app = new Vue({
//         el: '#app',
//         data: {
//             attendees: [{ name: '', email: '' }],
//             cost: 9.99,
//         },
//         computed: {
//             quantity: function () {
//                 return this.attendees.length;
//             },
//             checkoutTotal: function () {
//                 return this.cost * this.quantity;
//             }
//         },
//         methods: {
//             addAttendee: function (event) {
//                 event.preventDefault();
//                 this.attendees.push({
//                     name: '',
//                     email: '',
//                 });
//             },
//             removeAttendee: function (index) {
//                 this.attendees.splice(index, 1);
//             }
//         }
//     });
// })();