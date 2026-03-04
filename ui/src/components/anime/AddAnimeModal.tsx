import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';

import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { useAnime } from '@/hooks/query/useAnime';
import { CreateAnimeRequestDto } from '@/types';

interface AddAnimeModalProps {
  isOpen: boolean;
  onClose: () => void;
  contextId: string;
}

const formSchema = z.object({
  name: z.string().min(1, { message: 'Anime name is required.' }),
  description: z.string().min(1, { message: 'Description is required.' }),
  imageUrl: z.string().url({ message: 'Must be a valid URL.' }).optional().or(z.literal('')),
  externalLinks: z.string().optional(), // Comma-separated URLs
});

export function AddAnimeModal({ isOpen, onClose, contextId }: AddAnimeModalProps) {
  const { createAnime, isCreating } = useAnime(contextId);

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: '',
      description: '',
      imageUrl: '',
      externalLinks: '',
    },
  });

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    const newAnime: CreateAnimeRequestDto = {
      name: values.name,
      description: values.description,
      externalLinks: values.externalLinks
        ?.split(',')
        .map((link) => link.trim())
        .filter((link) => link.length > 0 && z.string().url().safeParse(link).success),
    };

    createAnime(newAnime, {
      onSuccess: () => {
        form.reset();
        onClose();
      },
    });
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-[425px] bg-surface text-text border-border rounded-xl shadow-2xl">
        <DialogHeader>
          <DialogTitle className="text-2xl font-bold text-primary">Add New Anime</DialogTitle>
          <DialogDescription className="text-muted-foreground">
            Enter details for the new anime item.
          </DialogDescription>
        </DialogHeader>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="grid gap-4 py-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Name</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Attack on Titan"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Description</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="A brief description of the anime."
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary resize-y min-h-[80px]"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="imageUrl"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">Image URL</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="https://example.com/image.jpg"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="externalLinks"
              render={({ field }) => (
                <FormItem>
                  <FormLabel className="text-text">External Links (comma-separated URLs)</FormLabel>
                  <FormControl>
                    <Textarea
                      placeholder="https://myanimelist.net/anime/16498, https://crunchyroll.com/series/attack-on-titan"
                      {...field}
                      className="bg-background border-border text-text focus-visible:ring-primary resize-y min-h-[80px]"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <DialogFooter className="pt-4">
              <Button
                type="submit"
                disabled={isCreating}
                className="bg-primary hover:bg-primary/90 text-primary-foreground rounded-lg shadow-md transition-all duration-200 ease-in-out transform hover:scale-105"
              >
                {isCreating ? 'Adding...' : 'Add Anime'}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
