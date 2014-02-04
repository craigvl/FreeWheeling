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

                // Remove 'id' member to perpare for INSERT
                if (type === httpVerbs.POST) {
                    delete data['id'];
                }

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
                //alert('Save');
                var
                    type = httpVerbs.POST,
                    url = '/api/Expanded/' + data.id;

                //if (data.id > 0) {
                //    type = httpVerbs.PUT;
                //    url += '/' + data.id;
                //}
                
                return this.commit(type, url, data);
            }
        };

    _.bindAll(ds, 'del', 'save');

    return {
        save: ds.save,
        del: ds.del
    }

})();