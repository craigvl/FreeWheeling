var httpVerbs = {
    POST: 'POST',
    PUT: 'PUT',
    GET: 'GET',
    DEL: 'DELETE'
};

var extendDataService = (function () {

    var
        ds = {
            commit: function (type, url, data) {

                return $.ajax({
                    type: type,
                    url: url,
                    data: data,
                    dataType: 'json'
                });
            },

            del: function (data) {
                return this.commit(httpVerbs.DEL, '/api/Expanded/' + data.id);
            },

            save: function (data) {
             
                var
                    type = httpVerbs.POST,
                    url = '/api/Expanded';
              
                return this.commit(type, url, data);
            }
        };

    _.bindAll(ds, 'del', 'save');

    return {
        save: ds.save,
        del: ds.del
    }

})();