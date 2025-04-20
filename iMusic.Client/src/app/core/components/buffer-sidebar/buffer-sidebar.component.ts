import { Component, OnInit } from '@angular/core';
import { Subject } from 'rxjs';
import { applicationRoleConstant } from '../../constants';
import { Router } from '@angular/router';
import { AudioPlayerService } from '../../services/audio-player.service';
import { SongInterface } from '../../interfaces/songs/song.interface';
import { takeUntil } from 'rxjs/operators';
import { SidebarService } from '../../services';

@Component({
  selector: 'mus-buffer-sidebar',
  templateUrl: './buffer-sidebar.component.html',
  styleUrls: ['./buffer-sidebar.component.scss']
})
export class BufferSidebarComponent implements OnInit {

  private readonly unsubscribe$: Subject<void> = new Subject<void>();
  public songs: SongInterface[] = [];
  public playingSong: SongInterface;

  public readonly applicationRole = applicationRoleConstant;
  public isOpen: boolean = false;

  constructor(private readonly router: Router,
    private readonly sidebarService: SidebarService,
    private readonly audioPlayerService: AudioPlayerService) {
  }

  public ngOnInit(): void {
    this.handleSidebarState();
    this.getSongs();
    this.getPlayingSong();

  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  private handleSidebarState(): void {
    this.sidebarService.isSidebarBufferOpened$
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe((isOpen): void => {
        this.isOpen = isOpen;
      });
  }

  private getSongs(): void {
    this.audioPlayerService.bufferSongs$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((songs): void => {
      this.songs = songs;
    });
  }

  private getPlayingSong(): void {
    this.audioPlayerService.currentSong$
    .pipe(takeUntil(this.unsubscribe$))
    .subscribe((song): void => {
      this.playingSong = song;
    });
  }

  public play(song: SongInterface): void{
    this.audioPlayerService.setCurrentSong(song);
  }

}
