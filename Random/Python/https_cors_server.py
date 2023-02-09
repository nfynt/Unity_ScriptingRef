#!/usr/bin/env python3
#!Author: Nfynt
#Generate ssl cert/key: openssl req -x509 -newkey rsa:2048 -keyout key.pem -out cert.pem -days 365
​
import http.server
import socketserver
import ssl
​
PORT = 8080
IP = "localhost"
​
class CORSRequestHandler(http.server.SimpleHTTPRequestHandler):
    extensions_map = {
        '': 'application/octet-stream',
        '.manifest': 'text/cache-manifest',
        '.html': 'text/html',
        '.png': 'image/png',
        '.jpg': 'image/jpg',
        '.svg':	'image/svg+xml',
        '.css':	'text/css',
        '.js':'application/x-javascript',
        '.wasm': 'application/wasm',
        '.json': 'application/json',
        '.xml': 'application/xml',
    }
    def end_headers (self):
        self.send_header("Access-Control-Allow-Credentials", "true")
        self.send_header("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time")
        self.send_header("Access-Control-Allow-Methods", "GET, POST, OPTIONS")
        self.send_header("Access-Control-Allow-Origin", "*")
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        #self.send_header("Cross-Origin-Resource-Policy", "same-site")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        http.server.SimpleHTTPRequestHandler.end_headers(self)
​
httpd = socketserver.TCPServer((IP, PORT), CORSRequestHandler)
httpd.socket = ssl.wrap_socket (httpd.socket, 
        keyfile="../key.pem", 
        certfile='../cert.pem', server_side=True)
​
try:
    print(f"serving at https://{IP}:{PORT}")
    httpd.serve_forever()
except KeyboardInterrupt:
    pass
