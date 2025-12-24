import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[appPhoneMask]',
    standalone: true
})
export class PhoneMaskDirective {

    constructor(private el: ElementRef<HTMLInputElement>) { }

    @HostListener('input', ['$event'])
    onInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const formatted = this.formatPhone(input.value);
        input.value = formatted;
    }

    @HostListener('keydown', ['$event'])
    onKeyDown(event: KeyboardEvent): void {
        // Sadece rakam, backspace, delete, tab, arrow tuşlarına izin ver
        const allowedKeys = ['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight', 'Home', 'End'];

        if (allowedKeys.includes(event.key)) {
            return;
        }

        // Sadece rakamlar
        if (!/^\d$/.test(event.key)) {
            event.preventDefault();
            return;
        }

        // 11 rakamdan fazla girişi engelle
        const input = event.target as HTMLInputElement;
        const currentDigits = input.value.replace(/\D/g, '');
        if (currentDigits.length >= 11) {
            event.preventDefault();
        }
    }

    private formatPhone(value: string): string {
        // Sadece rakamları al
        let digits = value.replace(/\D/g, '');

        // Maksimum 11 rakam
        if (digits.length > 11) {
            digits = digits.substring(0, 11);
        }

        // Eğer 5 ile başlıyorsa, başına 0 ekle
        if (digits.length > 0 && !digits.startsWith('0') && digits.startsWith('5')) {
            digits = '0' + digits;
            if (digits.length > 11) {
                digits = digits.substring(0, 11);
            }
        }

        // Format: 0XXX XXX XXXX
        if (digits.length === 0) {
            return '';
        } else if (digits.length <= 4) {
            return digits;
        } else if (digits.length <= 7) {
            return `${digits.slice(0, 4)} ${digits.slice(4)}`;
        } else {
            return `${digits.slice(0, 4)} ${digits.slice(4, 7)} ${digits.slice(7)}`;
        }
    }
}
