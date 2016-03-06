# prometheus-demo
A simple .Net demo of the basic capabilities of [Prometheus](https://prometheus.io/).

## Overview
This repository contain a minimal example of how to export metrics to Prometheus.

```
Prometheus 

An open-source service monitoring system and time series database. 
```

Prometheus use a pull mechanism to retrive metrics from nodes. The demo uses [Nancy](http://nancyfx.org/) to open a a port delivering the metrics over the plain text protocol.

The demo will show how to run with one windows box running the exporter, and one linux box running the server and [Grafana](http://grafana.org/) as docker containers.

## How to run

### Start up a linux box

These instruction a based on Ubuntu 14.4 LTS. Start the box and install [Docker](https://docs.docker.com/engine/installation/linux/ubuntulinux/). 

If you run this from as a virtual machine remember to use bridged networking so the machines can communicate. 

Alternatively if you have [Vagrant](https://www.vagrantup.com/), and [VirtualBox](https://www.virtualbox.org/) already installed just do: 

```
$ git clone https://github.com/ktrance/vagrant-docker
$ cd vagrant-docker
$ vagrant up
```

### Update prometheus.yml file

On the Ubuntu machine create a file at /tmp/prometheus.yml with the content below. 

```
scrape_configs:
  - job_name: 'prometheus_demo'
    scrape_interval: 5s
    scrape_timeout: 10s
    target_groups:
      - targets: ['localhost:9090']
```

REMEMBER to change the line :

```
targets: ['localhost:9090'] 
```

To the ip address and port of the windows machine where you will be running the exporter ex:

```
targets: ['172.56.56.12:9999'] 
```

### Start the Prometheus docker container

In a terminal on the linux machine run:

```
docker run -d -p 9090:9090 -v /tmp/prometheus.yml:/etc/prometheus/prometheus.yml prom/prometheus
```

Verify that Prometheus is up and pointing to the Windows machine by opening a browser pointing to http://UBUNTU_IP:9090

Once the page has opened go to the Status tab and check the configuration. The endpoint will be marked as down but we will fix that next.

![Prometheus Status](https://github.com/ktrance/prometheus-demo/raw/master/img/prometheus_status.PNG)

### Start the exporter

Clone this repository.

```
$ git clone https://github.com/ktrance/prometheus-demo.git
```

Open the project in Visual Studio, restore packages, then run. Once the collector is running go back to the Prometheus status page and see that the endpoint is found.

![Prometheus Status Up](https://github.com/ktrance/prometheus-demo/raw/master/img/prometheus_status_up.PNG)

Once its verified that the endpoint is running go to the Graph tab, select a counter and execute, you should start seeing the data flowing in.

![Prometheus Graph](https://github.com/ktrance/prometheus-demo/raw/master/img/prometheus_graph.PNG)

Read the documentation of the Prometheus [Query Language](https://prometheus.io/docs/querying/basics/) and [Query Examples](https://prometheus.io/docs/querying/examples/) to start playing around with the data.  

### Setup Grafana

On the Ubuntu machine open a terminal and run:
```
$ docker run -d -p 3000:3000 grafana/grafana
```

Login to Grafana in a browser pointing to http://UBUNTU_IP:3030, the user name and password is admin.

#### Change timestamping to utc

The first thing you need to do is change the timestamping to utc. This may not be necessary depending on where you are located in the world but if you, like me, are sitting in a timezone that uses daylight savings you wont get data otherwise.

In the grafana homepage click the cogwheel next to the dashboard selector and choose settings:

![Grafana Status](https://github.com/ktrance/prometheus-demo/raw/master/img/grafana_settings.PNG)

Flip the timezone from browser to utc:

![Grafana Utc](https://github.com/ktrance/prometheus-demo/raw/master/img/grafana_settings_utc.PNG)

#### Setup data source

We need to add Prometheus as a data source to enable Grafana to talk to it so go to Data sources -> Add new and configure it like this:

![Grafana Data Sources](https://github.com/ktrance/prometheus-demo/raw/master/img/grafana_datasource.PNG)

Test the connection and you are good to go.

#### Setup a simple dashboard

Go back to the Grafana main page by selecting Dashboards. In the drop down where it shows Home select New, this will create a new empty dashboard.

In the new dashboard click the small green button in the first row and add a new graph panel:

![Grafana Panel](https://github.com/ktrance/prometheus-demo/raw/master/img/grafana_panel.png)

Now you can start playing around with the data and queries by editing the panel. You edit the panel by clicking the panel title and choose edit.

![Grafana Query](https://github.com/ktrance/prometheus-demo/raw/master/img/grafana_query.PNG)

In the legend format you can use he labels that the exporter exports. In our example we exports the label process, machine an id so to use the process name as the legend you can use the following format:
```
{{process}}
```

Multiple labels can be combined so if you would like to have the legend process name - process id use you could specify it like this:
```
{{process}} - {{id}}
```

### Whats next

Well, its time to read up on the documentation and start experimenting. 
