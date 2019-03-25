<template>
  <div id="dashboard">
    <Filters :columns="columns" @updatedColumns="updateColumns"></Filters>
    <DashboardTable :records="records" :columns="columns"></DashboardTable>
  </div>
</template>

<script>
import Filters from './Filters.vue';
import DashboardTable from './DashboardTable.vue';
import * as signalR from '@aspnet/signalr';
import axios from "axios";

export default {
    name: 'Dashboard',
    components: {
        Filters,
        DashboardTable
    },
    data() {
        return {
            loading: false,
            records: [],
            columns: {
                "SitecoreInstanceName":true,
                "Event":true,
                "ItemId":true,
                "ItemName":true,
                "User":true,
                "TemplateName":true,
                "EventOrigin":true,
                "Timestamp":true
            },
            events: {
                
            }
        }
    },
    computed: {
        filteredRecords: function() {
            var rlist = [];
            
        }
    },
    created() {
        this.retrieveRecords();
        this.subscribeSignalR();
    },
    methods: {
        retrieveRecords(){
            this.loading = true;
            axios.get("https://audit-trail.azurewebsites.net/api/recent?code=iUKAj2y4nkWfVttWtzptC5Z1omTmpZrIZ26/6YZt2omB8cMBP/NB0w==")
            .then(response => {
                this.loading = false;
                this.records = response.data;
            })
            .catch(error => {
                this.loading = false;
                console.log(error);
            })
        },

        subscribeSignalR(){
            getConnectionInfo().then(info => {
                // make compatible with old and new SignalRConnectionInfo
                info.accessToken = info.accessToken || info.accessKey;
                info.url = info.url || info.endpoint;
                const options = {
                    accessTokenFactory: () => info.accessToken
                };

                const connection = new signalR.HubConnectionBuilder()
                    .withUrl(info.url, options)
                    .configureLogging(signalR.LogLevel.Information)
                    .build();

                connection.on('GeneralEventUpdate', this.updateRecords);
                connection.onclose(() => console.log('disconnected'));

                console.log('connecting...');
                connection.start()
                    .then(() => console.log('connected!'))
                    .catch(console.error);

            }).catch(alert);

            function getAxiosConfig() {
                const config = {
                    headers: {}
                };
                return config;
            }

            function getConnectionInfo() {
                return axios.post('https://audit-trail.azurewebsites.net/api/GeneralHubSubscribe', null, getAxiosConfig())
                    .then(resp => resp.data);
            }
        },

        updateRecords(parameters) {
            var newRecords = JSON.parse(parameters);
            for (var record in newRecords) {
                this.records.unshift(newRecords[record]);
            }
        },

        updateColumns(newColumns) {
            this.columns = newColumns;
        }
    }

    
}
</script>


<style>
#dashboard {
    width: 85%;
    margin:auto;
}
</style>