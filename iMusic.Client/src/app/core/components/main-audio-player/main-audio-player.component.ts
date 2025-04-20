import { Component, OnDestroy, OnInit } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import * as moment from "moment";
import { takeUntil } from 'rxjs/operators';
import { applicationRoleConstant } from '../../constants';
import { UserInterface } from '../../interfaces';
import { AuthService, SidebarService, StorageService } from '../../services';
import { UserApiService } from '../../services/api/user-api.service';
import { AudioPlayerService } from '../../services/audio-player.service';
import { SongInterface } from '../../interfaces/songs/song.interface';


@Component({
  selector: 'mus-main-audio-player',
  templateUrl: './main-audio-player.component.html',
  styleUrls: ['./main-audio-player.component.scss']
})
export class MainAudioPlayerComponent implements OnInit , OnDestroy {
  private readonly unsubscribe$: Subject<void> = new Subject<void>();

  public audio = new Audio();
  public musicLength: string = '0:00';
  public duration: number = 1;
  public currentTime: string = '0:00';
  public trackPointer: number;
  public currentSong: SongInterface;
  public musicList: SongInterface[] = [];
  public isFirstPlaying: boolean = true;
  public isLastPlaying: boolean = true;
  public isOpenBuffer: boolean = false;

  public readonly applicationRole = applicationRoleConstant;
  public currentUser: UserInterface;

  constructor(private readonly authService: AuthService,
    private readonly storageService: StorageService,
    private readonly userApiService: UserApiService,
    private readonly audioPlayerService: AudioPlayerService,
    private readonly sidebarBufferService: SidebarService
    ) {}

  public ngOnInit(): void {
    this.setControlTime();
    this.getCurrentUser();
    this.getBuffer();
    this.getCurrentSong();
    this.checkFirst();
    this.checkLast();
    this.checkSidebarBuffer();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  public checkSidebarBuffer(): void{
    this.sidebarBufferService.isSidebarBufferOpened$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((isOpen): void => {
      this.isOpenBuffer = isOpen;
    });
  }

  public setControlTime(): void {
    this.audio.ondurationchange = () => {
      const totalSeconds = Math.floor(this.audio.duration),
            duration = moment.duration(totalSeconds, 'seconds');
      this.musicLength = duration.seconds() < 10 ? 
                         `${Math.floor(duration.asMinutes())}:
                          0${duration.seconds()}` : 
                         `${Math.floor(duration.asMinutes())}:
                          ${duration.seconds()}`;
      this.duration = totalSeconds;
      if(this.currentTime == this.musicLength){
        this.next();
      }
    }

    this.audio.ontimeupdate = () => {
      const duration = moment.duration(
        Math.floor(this.audio.currentTime), 'seconds');
      this.currentTime = duration.seconds() < 10 ? 
                         `${Math.floor(duration.asMinutes())}:
                          0${duration.seconds()}` : 
                         `${Math.floor(duration.asMinutes())}:
                          ${duration.seconds()}`;
        if(this.currentTime == this.musicLength){
            this.next();
        }
      }

  }

  public getBuffer(): void {
    this.audioPlayerService.bufferSongs$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((songs): void => {
      this.musicList = songs;
    });
  }

  public getCurrentSong(): void {
    this.audioPlayerService.currentSong$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((song): void => {
        this.currentSong = song;

        if (!this.audio.paused) {
          this.audio.pause();
        }
        
        if(!!song){
          this.findTrackPointer();
           this.audio.src = this.currentSong.songUrl;
           this.audio.play();

           this.checkFirst();
           this.checkLast();
        }
      });
  }
  
  play(index?: number): void {
    if(!this.currentSong || this.musicList.length === 0){
      return;
    }
    if (index === undefined) {
      if (this.audio.paused) {
        if (this.audio.readyState === 0) {
          this.findTrackPointer();
          this.currentSong  =  this.currentSong ?? this.musicList[0];
          this.audio.src = this.currentSong.songUrl;
        }
        this.audio.play();
      } else {
        this.audio.pause();
      }
    } else {
      this.trackPointer = index;
      this.currentSong = this.musicList[index];
      this.audio.src = this.currentSong.songUrl;
      this.audio.play();
    } 
  }

  prev(): void {
    if(this.isFirstPlaying){
      return;
    }
    this.trackPointer--;
    this.currentSong = this.musicList[this.trackPointer];
    this.audioPlayerService.setCurrentSong(this.currentSong);
  }

  next(): void {
    if(this.isLastPlaying){
      return;
    }
    this.trackPointer++;
    this.currentSong = this.musicList[this.trackPointer];
    this.audioPlayerService.setCurrentSong(this.currentSong);
  }

  volumeSlider(event: any) {
    this.audio.volume = event.value;
  }

  durationSlider(event: any) {
    this.audio.currentTime = event.value;
  }


  private findTrackPointer(){
    const isCurrentSong = (element : SongInterface) => element.id === this.currentSong.id;

    this.trackPointer = this.musicList?.findIndex(isCurrentSong);
  }

  private checkFirst(): void {
    if(!this.trackPointer || this.trackPointer <= 0)
    {
      this.isFirstPlaying = true;
    }
    else
    {
      this.isFirstPlaying = false;
    }
  }

  private checkLast(): void {
    if(this.trackPointer == null || this.trackPointer == undefined){
      return;
    }
    if( this.trackPointer >= (this.musicList?.length - 1))
    {
      this.isLastPlaying = true;
    }
    else
    {
      this.isLastPlaying = false;
    }
  }

  public openSidebarBuffer(state: boolean): void{
    this.sidebarBufferService.isSidebarBufferOpened$.next(state);
  }

  public getCurrentUser(): void {
    this.authService.user$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((currentUser): void => {
        this.currentUser = currentUser;
      });
  }

  public logout(): void {
    this.authService.logOut();
  }
}
