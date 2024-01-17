export class Authentication {

  private apiUrl = 'http://localhost:5093/api/Home'; // .NET Core API URL'niz

  constructor(private http: HttpClient) { }

  public login(credentials: UserModel):Observable<any> {
    return this.http.post<any>(this.apiUrl + '/Login', credentials);
  }

}
export class SignalRService {
  private hubConnection: signalR.HubConnection | undefined;

  public startConnection = (token:string) => {
    /* console.log(token);
     debugger;*/
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5093/chat',
          {accessTokenFactory: () => token
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('Connection Started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }

  public sendMessage = (user: string, message: string) => {
    // @ts-ignore
    this.hubConnection.invoke('SendMessage', user, message)
      .catch(err => console.error(err.message));
  }

  public addReceiveMessageListener = (callback: (user: string, message: string) => void) => {
    // @ts-ignore
    this.hubConnection.on('ReceiveMessage', (user, message) => {
      callback(user, message);
    });
  }


  import {Component, OnInit} from '@angular/core';
import {CommonModule, NgForOf} from '@angular/common';
import { RouterOutlet } from '@angular/router';
import {SignalRService} from "./service/SignalRService";
import {FormsModule} from "@angular/forms";
import {Authentication} from "./service/authentication";
import {HttpClientModule} from "@angular/common/http";
import {UserModel} from "./model/UserModel";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: true,
  imports: [
    FormsModule,
    NgForOf,
    HttpClientModule
  ],
  styleUrl: './app.component.css'
})
export class AppComponent implements  OnInit{
  messages: { sender: string; text: string; }[] = [];
  newMessage = '';
  token:string="";
  user: UserModel = {
    Username: 'kullaniciadi',
    Password: 'kullaniciadi'
  };
  constructor(private signalRService: SignalRService,private authService:Authentication) {
    // @ts-ignore

   // const credentials = { username: 'user', password: 'pass' };

  }

  ngOnInit() {
    this.authService.login(this.user).subscribe(
      data => {
       /* console.log(data);
        debugger;*/
        //this.token=data.token;
        this.signalRService.startConnection(data.token);
      },
      error => {
        console.error('Login failed', error);
      }
    );
    /*console.log(this.token);
    debugger;*/

    this.signalRService.addReceiveMessageListener((user, message) => {
      this.messages.push({ sender: user, text: message });
    });

  }

  sendMessage() {
    this.signalRService.sendMessage('KullanıcıAdı', this.newMessage);
    this.newMessage = ''; // Metin kutusunu temizle
  }
}

<div class="chat-container">
  <!-- Mesajları Listeleme -->
  <div class="messages">
    <div *ngFor="let message of messages">
      <strong>{{ message.sender }}:</strong> {{ message.text }}
    </div>
  </div>

  <!-- Mesaj Gönderme Formu -->
  <div class="send-message-form">
    <input [(ngModel)]="newMessage" type="text" placeholder="Mesajınızı yazın"/>
    <button (click)="sendMessage()">Gönder</button>
  </div>
</div>
