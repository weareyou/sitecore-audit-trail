<template>
  <div id="dashboard">
    <Filters :columns="columns" @toggleColumn="toggleColumn" @changeFilter="changeFilter"></Filters>
    <DashboardTable :records="filteredRecords()" :columns="columns"></DashboardTable>
    <infinite-loading @infinite="retrieveRecords" :distance="70"></infinite-loading>
  </div>
</template>

<script>
import Filters from './Filters.vue';
import DashboardTable from './DashboardTable.vue';
import * as signalR from '@aspnet/signalr';
import axios from "axios";
import InfiniteLoading from 'vue-infinite-loading';

export default {
    name: 'Dashboard',
    components: {
        Filters,
        DashboardTable,
        InfiniteLoading
    },
    data() {
        return {
            loading: false,
            continuationToken: "",
            records: [],
            // TODO: populate columns&recordfilters dynamically
            columns: {
                "SitecoreInstanceName":true,
                "Event":true,
                "ItemId":false,
                "ItemName":true,
                "User":true,
                "TemplateName":true,
                "EventOrigin":true,
                "Timestamp":true
            },
            // key = fieldname, value = match filter
            recordFilters: {
            }
        }
    },
    created() {
        //this.retrieveRecords();
        this.subscribeSignalR();
    },
    methods: {
        filteredRecords: function() {
            if (Object.keys(this.recordFilters).length < 1) {
                return this.records;
            }

            var frecords = [];
            for (var record in this.records) {
                var pass = true;
                for (var filter in this.recordFilters) {
                    console.log(this.records[record][filter]);
                    if (this.records[record][filter] != undefined && this.records[record][filter] != "") {
                        
                        if (!(this.records[record][filter].includes(this.recordFilters[filter]))) {
                            pass = false;
                        } 
                    }
                }

                if (pass === true) {
                    frecords.push(this.records[record]);
                }

          }
          return frecords;
            
        },
        retrieveRecords($state){
            this.loading = true;
            var pageSize = "30";
            var route = process.env.VUE_APP_API_DOMAIN + "/api/recent/";
            var apiKey = process.env.VUE_APP_API_KEY;

            var url = route + pageSize + "?" + apiKey;
            if (this.continuationToken != "") {
                url = route + pageSize + "/" + encodeURIComponent(this.continuationToken) + "?" + apiKey;
            }


            axios.get(url)
            .then(response => {
                this.loading = false;
                if (this.records.length < 1) {
                    this.records = response.data;
                } else {
                    this.records.push(...response.data);    
                } 
                this.continuationToken = response.headers.continuationtoken;
                $state.loaded();    
            })
            .catch(error => {
                this.loading = false;
                $state.complete();
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
                return axios.post(process.env.VUE_APP_API_DOMAIN + "/api/GeneralHubSubscribe", null, getAxiosConfig())
                    .then(resp => resp.data);
            }
        },

        updateRecords(parameters) {
            var newRecords = JSON.parse(parameters);
            for (var record in newRecords) {
                this.records.unshift(newRecords[record]);
            }
        },

        toggleColumn(columnName) {
            this.columns[columnName] = !this.columns[columnName];
        },

        changeFilter(fieldName, value) {
            if (value === "" || value == null) {
                delete this.recordFilters[fieldName];
            } else {
                this.recordFilters[fieldName] = value;
            }
            this.$forceUpdate();
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