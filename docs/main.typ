#align(center, heading("Owf Flow", level: 1))
#align(center, "Project Requirements Document")

#pagebreak()

#align(center, heading("Overview", level: 1))
#par(none)
Owl Flow is a modern load balancer designed to efficiently distribute network traffic across multiple servers, 
improving performance, reliability, and scalability.



#heading("Key Features of Owl Flow", level: 2)
+ *Resource Optimization* – Evenly distributes workload among servers to prevent overload.
+ *High Availability* – Automatically reroutes traffic if a server fails.
+ *Improved Performance* – Reduces response time by intelligently distributing requests.
+ *Scalability* – Easily add new servers to the pool without downtime.

#heading("Supported Load Balancing Types", level: 2)
+ *Layer 7 (Application Layer)* – HTTP/HTTPS-based balancing, ideal for web applications.
+ *GSLB (Global Server Load Balancing)* – Geographic traffic distribution for global networks.

#heading("Load Balancing Algorithms", level: 2)

Owl Flow supports multiple distribution strategies:
+ *Round Robin* – Requests are cycled between servers in sequence.
+ *Least Connections* – Traffic is sent to the server with the fewest active connections.
+ *IP Hash* – A client is consistently directed to the same server (useful for sessions).
+ *Weighted Round Robin* – Servers handle traffic according to their capacity (weights).
+ *Least Response Time* – Requests are routed to the fastest-responding server.
#pagebreak()

#heading("Functional Requirements", level: 1)
#parbreak()

#heading("Web Interface", level: 2)

#heading("Authentication", level: 3)

As an Admin, I want to sign in to the web interface using:
- Email

- Password

so that I can access the load balancer dashboard.

As an Admin, I want to receive an error message when entering incorrect credentials.

As an Admin, I want to sign out of the web interface to secure my session.

#heading("Load Balancer Management", level: 3)

As an Admin, I want to:

- Create a new load balancer (LB) instance

- Specify its name (e.g., "Production-LB")

- Select a default algorithm (Round Robin/Least Connections/etc.)
so that I can start distributing traffic.

As an Admin, I want to delete an existing LB when it’s no longer needed.

#heading("Server Management", level: 3)

As an Admin, I want to:

- Add a backend server to an LB by entering:

 - IP address/hostname

 - Port (e.g., 80, 443)

 - Optional weight (for Weighted Round Robin) so that traffic can be routed to it.

As an Admin, I want to remove a server from the LB pool during maintenance.

As an Admin, I want to temporarily disable a server (without deleting it) for troubleshooting.

#heading("Monitoring & Metrics", level: 3)

As an Admin, I want to see real-time statistics for each LB, including:

- Total requests per second (RPS)

- Average response time (in ms)

- Error rates (4xx/5xx) per server
so that I can assess performance.

As an Admin, I want to view a live list of all servers with their:

- Current status (Online/Offline/Overloaded)

- CPU/RAM usage (if metrics are available)

- Active connections count
so that I can identify bottlenecks.

As an Admin, I want to configure health check intervals (e.g., every 1-5m) and thresholds (e.g., 3 critical errors = offline).

#heading("Alerts & Notifications", level: 3)

As an Admin, I want to receive browser notifications (or emails) when:

- A server fails health checks.

- Average latency exceeds a threshold (e.g., 500ms).

- Error rate spikes above 5%.

#heading("UI/UX Requirements", level: 3)

As an Admin, I want to:

- See a dashboard with all LBs and their statuses (Green/Yellow/Red).

- Click on an LB to view detailed server metrics in a table/graph.

- Toggle between algorithms (e.g., switch from Round Robin to Least Connections) without downtime.

#heading("Example Edge Cases", level: 3)

As an Admin, I want to:

- See a warning when adding a duplicate server IP.

- Receive an error if I try to delete an LB that’s still handling active traffic.

- View historical metrics (last 24h) to analyze trends.

#pagebreak()

#heading("Non-functional Requirements", level: 1)

#heading("Technologies", level: 2)
#table(
  columns: (1fr, 3fr),
   fill:  rgb("#d1d1d1"),
  stroke: none,
  align: left,
  inset: 8pt,
  [Back-end],    [ASP.NET Core],
  [Front-end],   [React + TypeScript],
  [API],         [REST + SignalR],
  [Database],    [PostgreSQL],
  [Caching],     [Redis],
  [Infrastructure], [Docker + Kubernetes]
)
#par(none)



#heading("Requirements", level: 2)
+ *Performance*:

    - Handle ≥ 1000 RPS on mid-range hardware

    - < 50ms latency for 95% of requests

+ *Security*:

    - HTTPS support (Let's Encrypt)

    - DDoS protection (rate limiting)

    - Admin panel authentication

+ *Scalability*:

    - Horizontal scaling (adding new nodes)~~

    - Geo-distributed server support (GSLB)
#par(none)
#heading("Risks and Challenges", level: 2)
+ *DDoS Attacks*
+ *Load Balancer Bottleneck*
+ *Configuration Errors*: Broken routing after updates.